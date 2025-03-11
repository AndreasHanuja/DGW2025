using Game.Map.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Map.WFC
{
    public class WFCModel : MonoBehaviour
    {
        public event Action<Vector2Int, short> OnOutputChange;

        public const int worldSize = 32;
        private List<PlyModelPrefab> prefabs;
        private byte[,] inputData = new byte[worldSize, worldSize];
        private short[,] outputData = new short[worldSize, worldSize];

        public void SetPlyModelPrefabs(List<PlyModelPrefab> prefabs)
        {
            this.prefabs = prefabs;
        }
        public void SetInput(Vector2Int position, byte value)
        {
            inputData[position.x, position.y] = value;
        }
        public short GetOutput(Vector2Int position)
        {
            return outputData[position.x, position.y];
        }
        public void SetOutput(Vector2Int position, short value)
        {
            outputData[position.x, position.y] = value;
            OnOutputChange?.Invoke(position, value);
        }
        public byte GetInput(Vector2Int position)
        {
            return inputData[position.x, position.y];
        }
        public byte[,] GetInputData()
        {
            return inputData;
        }

        public void SetPrefabs(List<PlyModelPrefab> prefabs)
        {
            this.prefabs = prefabs.ToList();
        }
        public List<PlyModelPrefab> GetPrefabs()
        {
            return prefabs;
        }
    }
}