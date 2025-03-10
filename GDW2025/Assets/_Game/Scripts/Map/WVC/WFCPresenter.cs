using Game.Map.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Game.Map.WFC
{
    public class WFCPresenter : MonoBehaviour
    {
        public event Action<PlyModel> OnModelUpdated; 

        public static WFCPresenter Instance;
        private WFCView view;
        private WFCModel model;

        private void Awake()
        {
            view = GetComponent<WFCView>();
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

        private void OutputUpdateHandler(Vector3Int position, byte output)
        {
            OnModelUpdated?.Invoke(model.GetPrefabs()[output].Instantiate(position));
        }

        public void SetInput(Vector3Int position, byte value)
        {
            model.SetInput(position, value);
            RecalculateOutput(model.GetInputData(), model.GetPrefabs());
        }

        public byte GetInput(Vector3Int position)
        {
            return model.GetInput(position);
        }

        public void SetPlyModelPrefabs(List<PlyModelPrefab> prefabs)
        {
            model.SetPrefabs(prefabs);

            RecalculateOutput(model.GetInputData(), model.GetPrefabs());
        }

        private void RecalculateOutput(byte[,,] inputData, List<PlyModelPrefab> prefabs)
        {
            int size = WFCModel.worldSize;
            HashSet<byte>[,,] possibilities = new HashSet<byte>[size, size, size];

            Init(possibilities, inputData, prefabs);

            HashSet<Vector3Int> openPositions = new();
            InitPositions(openPositions, possibilities);
            HashSet<Vector3Int> uncollapsedPositions = openPositions.ToHashSet();

            while (uncollapsedPositions.Any())
            {
                Vector3Int currentValue;
                HashSet<byte> currentPossibilities;
                while (openPositions.Any())
                {
                    currentValue = openPositions.ElementAt(0);
                    openPositions.Remove(currentValue);

                    currentPossibilities = possibilities[currentValue.x, currentValue.y, currentValue.z];
                    foreach(byte possibility in currentPossibilities.ToList())
                    {
                        if (CheckPossibility(possibilities, possibility, currentValue))
                        {
                            continue;
                        }
                        currentPossibilities.Remove(possibility);
                        foreach(Vector3Int neighbor in GetNeighbours(currentValue))
                        {
                            openPositions.Add(neighbor);
                        }
                    }
                }
                currentValue = uncollapsedPositions.ElementAt(0);
                uncollapsedPositions.Remove(currentValue);
                CellCollapse(currentValue, possibilities[currentValue.x, currentValue.y, currentValue.z], openPositions);
            }

            Output(possibilities);
        } //TODO CALLEN

        private void Init(HashSet<byte>[,,] possibilities, byte[,,] inputData, List<PlyModelPrefab> prefabs)
        {
            int size = WFCModel.worldSize;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        possibilities[x, y, z] = new HashSet<byte>();
                        for (byte i = 0; i < prefabs.Count; i++)
                        {
                            if (prefabs[i].allowedInputs.Contains(inputData[x, y, z]))
                            {
                                possibilities[x, y, z].Add(i);
                            }
                        }
                    }
                }
            }
        }
        private void InitPositions(HashSet<Vector3Int> openPositions, HashSet<byte>[,,] possibilities)
        {
            int size = WFCModel.worldSize;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        if (possibilities[x, y, z].Count > 1)
                        {
                            openPositions.Add(new Vector3Int(x, y, z));
                        }    
                    }
                }
            }
        }
        private void CellCollapse(Vector3Int position, HashSet<byte> possibilities, HashSet<Vector3Int> openPositions)
        {
            List<PlyModelPrefab> prefabs = model.GetPrefabs();
            int priorityClass = prefabs[possibilities.First()].collapsePriority;

            possibilities.RemoveWhere(p => prefabs[p].collapsePriority != priorityClass);
            System.Random random = new System.Random(position.GetHashCode());
            byte selectedPossibility = (byte)random.Next(possibilities.Count);

            possibilities.Clear();
            possibilities.Add(selectedPossibility);

            foreach(Vector3Int neighbor in GetNeighbours(position))
            {
                openPositions.Add(neighbor);
            }
        }
        private bool CheckPossibility(HashSet<byte>[,,] possibilities, byte possibility, Vector3Int position)
        {
            List<PlyModelPrefab> prefabs = model.GetPrefabs();
            PlyModelPrefab myPrefab = prefabs[possibility];

            foreach (Vector3Int neighbor in GetNeighbours(position))
            {
                bool matchAny = false;
                foreach(byte possibilityNeighbor in possibilities[neighbor.x, neighbor.y, neighbor.z])
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
        private HashSet<Vector3Int> GetNeighbours(Vector3Int position)
        {
            HashSet<Vector3Int> output = new();
            output.Add(NeighbourHelper(position + Vector3Int.forward));
            output.Add(NeighbourHelper(position + Vector3Int.back));
            output.Add(NeighbourHelper(position + Vector3Int.left));
            output.Add(NeighbourHelper(position + Vector3Int.right));
            output.Add(NeighbourHelper(position + Vector3Int.up));
            output.Add(NeighbourHelper(position + Vector3Int.down));
            output.Remove(position);

            return output;
        }
        private Vector3Int NeighbourHelper(Vector3Int position)
        {
            return new Vector3Int(
                Mathf.Clamp(position.x, 0, WFCModel.worldSize - 1),
                Mathf.Clamp(position.y, 0, WFCModel.worldSize - 1),
                Mathf.Clamp(position.z, 0, WFCModel.worldSize - 1)
            );
        }
        private void Output(HashSet<byte>[,,] possibilities)
        {
            int size = WFCModel.worldSize;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        byte output = possibilities[x, y, z].First();
                        Vector3Int position = new Vector3Int(x, y, z);
                        if (model.GetOutput(position) != output)
                        {
                            model.SetOutput(position, output);
                        }
                    }
                }
            }
        }
    }
}
