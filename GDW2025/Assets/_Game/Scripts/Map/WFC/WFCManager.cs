using Game.Map.Models;
using Game.Map.Voxel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

namespace Game.Map.WFC
{
    public class WFCManager
    {
        #region SINGLETON
        public static WFCManager Instance = new WFCManager();
        private WFCManager() { }
        #endregion

        #region PROPERTIES
        private int mapSize;
        private int seed;
        private List<PlyModelPrefab> prefabs;

        private HashSet<short>[,] initialPossibilities;
        private byte[,] groundCached;
        private byte[,] inputCached;
        private short[,] outputCached;
        #endregion

        public List<PlyModelPrefab> GetPrefabs()
        {
            return prefabs.ToList();
        }
        public void WFC_Init(int mapSize, int seed, List<PlyModelPrefab> prefabs, byte[,] groundCache)
        {
            this.mapSize = mapSize;
            this.seed = seed;
            this.prefabs = prefabs;
            this.groundCached = groundCache;

            InitCaches();          
        }
        public IEnumerable<WFCOutputChange> WFC_Iterate(IEnumerable<WFCInputChange> inputs)
        {
            if(inputs.Any(i => inputCached[i.position.x, i.position.y] != 0)){
                return new List<WFCOutputChange>();
            }
            foreach (WFCInputChange input in inputs)
            {
                inputCached[input.position.x, input.position.y] = input.value;
            }

            InitInitialPossibilities();

            List<Vector2Int> uncollapsedPositions = new();
            HashSet<Vector2Int> propagatePositions = new();
            InitInitialPositions(uncollapsedPositions, propagatePositions);

            PropagateAll(propagatePositions, initialPossibilities);
            //HashSet<short>[,] possibilities = DeepCopyInitialPossibilities();
            while (uncollapsedPositions.Any())
            {
                PropagateAll(propagatePositions, initialPossibilities);
                CollapseBest(uncollapsedPositions, initialPossibilities, propagatePositions);
            }

            return Output(initialPossibilities);

            /*List<Vector2Int> uncollapsedPositions = new();
            HashSet<Vector2Int> propagatePositions = new();

            for(int x = 0; x<mapSize; x++)
            {
                for(int y = 0; y<mapSize; y++)
                {
                    uncollapsedPositions.Add(new Vector2Int(x, y));
                }
            }
            foreach(WFCInputChange input in inputs)
            {
                inputCached[input.position.x, input.position.y] = input.value;
                initialPossibilities[input.position.x, input.position.y] = GetBaseProbabilitySpace(input.position.x, input.position.y);
                propagatePositions.Add(input.position);
            }
            PropagateAll(propagatePositions, initialPossibilities);

            HashSet<short>[,] possibilities = DeepCopyInitialPossibilities();
            while (uncollapsedPositions.Any())
            {
                PropagateAll(propagatePositions, possibilities);
                CollapseBest(uncollapsedPositions, possibilities, propagatePositions);
            }
            return Output(possibilities);*/
        }

        #region HELPER FUNCTIONS
        private void InitCaches()
        {
            inputCached = new byte[mapSize, mapSize];
            outputCached = new short[mapSize, mapSize];

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    outputCached[x, y] = -1;
                }
            }
        }
        private void InitInitialPossibilities()
        {
            initialPossibilities = new HashSet<short>[mapSize, mapSize];

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    initialPossibilities[x, y] = GetBaseProbabilitySpace(x, y);
                }
            }
        }
        private HashSet<short> GetBaseProbabilitySpace(int x, int y)
        {
            return prefabs.Where(p => p.allowedGround.Contains(groundCached[x, y]) && p.allowedInputs.Contains(inputCached[x, y])).Select(p => (short)p.id).ToHashSet();
        }
        private void InitInitialPositions(List<Vector2Int> uncollapsedPositions, HashSet<Vector2Int> propagatePositions)
        {
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (initialPossibilities[x, y].Count <= 1)
                    {
                        //continue;
                    }
                    Vector2Int pos = new Vector2Int(x, y);
                    uncollapsedPositions.Add(pos);
                    propagatePositions.Add(pos);
                }
            }
        }
        private HashSet<short>[,] DeepCopyInitialPossibilities()
        {
            int dim0 = initialPossibilities.GetLength(0);
            int dim1 = initialPossibilities.GetLength(1);
            HashSet<short>[,] copy = new HashSet<short>[dim0, dim1];

            for (int i = 0; i < dim0; i++)
            {
                for (int j = 0; j < dim1; j++)
                {
                    copy[i, j] = new HashSet<short>(initialPossibilities[i, j]);
                }
            }
            return copy;
        }
        private void CollapseBest(List<Vector2Int> uncollapsedPositions, HashSet<short>[,] possibilities, HashSet<Vector2Int> propagatePositions)
        {
            int index = uncollapsedPositions.IndexOfMinBy(p => possibilities[p.x, p.y].Count);
            Vector2Int position = uncollapsedPositions[index];
            uncollapsedPositions.RemoveAt(index);

            if (possibilities[position.x, position.y].Count <= 1)
            {
                return;
            }

            System.Random random = new System.Random(position.GetHashCode() + seed);
            short selectedPossibility = possibilities[position.x, position.y].MaxBy(p => prefabs[p].weight * random.Next(10));

            possibilities[position.x, position.y].Clear();
            possibilities[position.x, position.y].Add(selectedPossibility);
            foreach (Vector2Int neighbor in GetNeighbours(position))
            {
                if (possibilities[neighbor.x, neighbor.y].Count > 1)
                {
                    propagatePositions.Add(neighbor);
                }
            }
        }
        private void PropagateAll(HashSet<Vector2Int> propagatePositions, HashSet<short>[,] possibilities)
        {
            Vector2Int currentValue;
            HashSet<short> currentPossibilities;

            while (propagatePositions.Any()) 
            {
                currentValue = propagatePositions.First();
                propagatePositions.Remove(currentValue);

                currentPossibilities = possibilities[currentValue.x, currentValue.y];

                foreach (short possibility in currentPossibilities.ToList())
                {
                    if (CheckPossibility(possibilities, possibility, currentValue))
                    {
                        continue;
                    }
                    currentPossibilities.Remove(possibility);
                    if(currentPossibilities.Count == 0)
                    {
                        currentPossibilities = GetBaseProbabilitySpace(currentValue.x, currentValue.y);
                        propagatePositions.Add(currentValue);
                        return;
                    }
                    foreach (Vector2Int neighbor in GetNeighbours(currentValue))
                    {
                        propagatePositions.Add(neighbor);
                    }
                }
            }
        }
        private bool CheckPossibility(HashSet<short>[,] possibilities, short possibility, Vector2Int position)
        {
            PlyModelPrefab myPrefab = prefabs[possibility];

            if(position.x == 0 && !myPrefab.CheckConnectivity(prefabs.Last(), Vector2Int.left))
            {
                return false;
            }
            if (position.x == mapSize - 1 && !myPrefab.CheckConnectivity(prefabs.Last(), Vector2Int.right))
            {
                return false;
            }
            if (position.y == 0 && !myPrefab.CheckConnectivity(prefabs.Last(), Vector2Int.down))
            {
                return false;
            }
            if (position.y == mapSize - 1 && !myPrefab.CheckConnectivity(prefabs.Last(), Vector2Int.up))
            {
                return false;
            }
            foreach (Vector2Int neighbor in GetNeighbours(position))
            {
                if (possibilities[neighbor.x, neighbor.y].Any(p => myPrefab.CheckConnectivity(prefabs[p], neighbor - position)))
                {
                    continue;
                }
                return false;
            }
            return true;
        }
        private IEnumerable<Vector2Int> GetNeighbours(Vector2Int position)
        {
            HashSet<Vector2Int> output = new();
            if (position.y < mapSize - 1)
            {
                yield return position + Vector2Int.up;
            }
            if (position.x < mapSize - 1)
            {
                yield return position + Vector2Int.right;
            }
            if (position.y > 1)
            {
                yield return position + Vector2Int.down;
            }
            if (position.x > 1)
            {
                yield return position + Vector2Int.left;
            }
        }
        private List<WFCOutputChange> Output(HashSet<short>[,] possibilities)
        {
            List<WFCOutputChange> changes = new();

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    short output = possibilities[x, y].First();
                    if (outputCached[x, y] != output) 
                    {
                        changes.Add(new WFCOutputChange() { position = new Vector2Int(x, y), oldValue = outputCached[x, y], newValue = output });
                        outputCached[x, y] = output;
                    }
                }
            }

            return changes;
        }
        #endregion
    }


    #region HELPER STRUCTS & ENUMS
    public enum ChangeType { Map, Input}
    public struct WFCInputChange
    {
        public ChangeType Type;
        public Vector2Int position;
        public byte value;
    }
    public struct WFCOutputChange
    {
        public Vector2Int position;
        public short oldValue;
        public short newValue;
    }
    #endregion
}