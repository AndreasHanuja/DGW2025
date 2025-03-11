using Game.Map.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Map.WFC
{
    public class WFCPresenter : MonoBehaviour
    {
        public event Action<PlyModel> OnModelUpdated; 

        public static WFCPresenter Instance;
        private WFCModel model;

        private void Awake()
        {
            model = GetComponent<WFCModel>();

            Instance = this;
        }

        private void OnEnable()
        {
            model.OnOutputChange += OutputUpdateHandler;
        }
        private void OnDisable()
        {
            model.OnOutputChange -= OutputUpdateHandler;
        }

        private void OutputUpdateHandler(Vector2Int position, short output)
        {
            position *= PlyModelPrefab.modelSize;
            OnModelUpdated?.Invoke(model.GetPrefabs()[output - 1].Instantiate(new Vector3Int(position.x, 0, position.y)));
        }

        public void SetInput(Vector2Int position, byte value)
        {
            model.SetInput(position, value);
            RecalculateOutput(model.GetInputData(), model.GetPrefabs());
        }

        public byte GetInput(Vector2Int position)
        {
            return model.GetInput(position);
        }

        public void SetPlyModelPrefabs(List<PlyModelPrefab> prefabs)
        {
            model.SetPrefabs(prefabs);

            RecalculateOutput(model.GetInputData(), model.GetPrefabs());
        }

        private void RecalculateOutput(byte[,] inputData, List<PlyModelPrefab> prefabs)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            int size = WFCModel.worldSize;
            HashSet<short>[,] possibilities = new HashSet<short>[size, size];

            Init(possibilities, inputData, prefabs);

            HashSet<Vector2Int> openPositions = new();
            InitPositions(openPositions, possibilities);
            HashSet<Vector2Int> uncollapsedPositions = openPositions.ToHashSet();

            while (uncollapsedPositions.Any())
            {
                Vector2Int currentValue;
                HashSet<short> currentPossibilities;
                while (openPositions.Any())
                {
                    currentValue = openPositions.ElementAt(0);
                    openPositions.Remove(currentValue);

                    currentPossibilities = possibilities[currentValue.x, currentValue.y];
                    foreach(byte possibility in currentPossibilities.ToList())
                    {
                        if (CheckPossibility(possibilities, possibility, currentValue))
                        {
                            continue;
                        }
                        currentPossibilities.Remove(possibility);
                        foreach(Vector2Int neighbor in GetNeighbours(currentValue))
                        {
                            openPositions.Add(neighbor);
                        }
                    }
                }
                currentValue = uncollapsedPositions.ElementAt(0);
                uncollapsedPositions.Remove(currentValue);
                CellCollapse(currentValue, possibilities[currentValue.x, currentValue.y], openPositions);
            }

            stopwatch.Stop();
            UnityEngine.Debug.Log("WFC took " + stopwatch.ElapsedMilliseconds+" ms");
            stopwatch.Restart();
            Output(possibilities);
            UnityEngine.Debug.Log("Generating mesh took " + stopwatch.ElapsedMilliseconds + " ms");
        }

        private void Init(HashSet<short>[,] possibilities, byte[,] inputData, List<PlyModelPrefab> prefabs)
        {
            int size = WFCModel.worldSize;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    possibilities[x, y] = new HashSet<short>();
                    for (byte i = 0; i < prefabs.Count; i++)
                    {
                        if (prefabs[i].allowedInputs.Contains(inputData[x, y]))
                        {
                            possibilities[x, y].Add(i);
                        }
                    }
                }
            }
        }
        private void InitPositions(HashSet<Vector2Int> openPositions, HashSet<short>[,] possibilities)
        {
            int size = WFCModel.worldSize;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (possibilities[x, y].Count > 1)
                    {
                        openPositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        private void CellCollapse(Vector2Int position, HashSet<short> possibilities, HashSet<Vector2Int> openPositions)
        {
            List<PlyModelPrefab> prefabs = model.GetPrefabs();
            System.Random random = new System.Random(position.GetHashCode());
            IEnumerable<(short, float)> weightsPos = possibilities.Select(p => (p, prefabs[p].weight * random.Next(0, 10000)));
            
            short bestPos = -1;
            float bestWeight = 0;
            foreach(var t in weightsPos)
            {
                if (t.Item2 > bestWeight)
                {
                    bestWeight = t.Item2;
                    bestPos = t.Item1;
                }
            }

            possibilities.Clear();
            possibilities.Add(bestPos);

            foreach(Vector2Int neighbor in GetNeighbours(position))
            {
                openPositions.Add(neighbor);
            }
        }
        private bool CheckPossibility(HashSet<short>[,] possibilities, short possibility, Vector2Int position)
        {
            List<PlyModelPrefab> prefabs = model.GetPrefabs();
            PlyModelPrefab myPrefab = prefabs[possibility];

            foreach (Vector2Int neighbor in GetNeighbours(position))
            {
                bool matchAny = false;
                foreach(byte possibilityNeighbor in possibilities[neighbor.x, neighbor.y])
                {
                    if (myPrefab.CheckConnectivity(model.GetPrefabs()[possibilityNeighbor], neighbor - position))
                    {
                        matchAny = true;
                        break;
                    }
                }
                if (!matchAny)
                {
                    return false;
                }
            }
            return true;
        }
        private HashSet<Vector2Int> GetNeighbours(Vector2Int position)
        {
            HashSet<Vector2Int> output = new();
            output.Add(NeighbourHelper(position + Vector2Int.up));
            output.Add(NeighbourHelper(position + Vector2Int.right));
            output.Add(NeighbourHelper(position + Vector2Int.down));
            output.Add(NeighbourHelper(position + Vector2Int.left));
            output.Remove(position);

            return output;
        }
        private Vector2Int NeighbourHelper(Vector2Int position)
        {
            return new Vector2Int(
                Mathf.Clamp(position.x, 0, WFCModel.worldSize - 1),
                Mathf.Clamp(position.y, 0, WFCModel.worldSize - 1)
            );
        }
        private void Output(HashSet<short>[,] possibilities)
        {
            List<(Vector2Int, byte)> changes = new ();
            int size = WFCModel.worldSize;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    byte output = (byte)(possibilities[x, y].First() + 1);
                    Vector2Int position = new Vector2Int(x, y);

                    if (model.GetOutput(position) != output)
                    {
                        changes.Add((position, output));
                    }
                }
            }

            Parallel.ForEach(changes, t => model.SetOutput(t.Item1, t.Item2));
        }
    }
}
