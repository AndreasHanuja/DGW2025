using Game.Map.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Map.WFC
{
    public class WFCModel : MonoBehaviour
    {
        public event Action<Vector3Int, byte> OnOutputChange;

        public const int worldSize = 16;
        private List<PlyModelPrefab> prefabs;
        private byte[,,] inputData = new byte[worldSize, worldSize, worldSize];
        private byte[,,] outputData = new byte[worldSize, worldSize, worldSize];

        public void SetPlyModelPrefabs(List<PlyModelPrefab> prefabs)
        {
            this.prefabs = prefabs;
        }
        public void SetInput(Vector3Int position, byte value)
        {
            inputData[position.x, position.y, position.z] = value;
        }
        public byte GetOutput(Vector3Int position)
        {
            return outputData[position.x, position.y, position.z];
        }
        public void SetOutput(Vector3Int position, byte value)
        {
            outputData[position.x, position.y, position.z] = value;
            OnOutputChange?.Invoke(position, value);
        }
        public byte GetInput(Vector3Int position)
        {
            return inputData[position.x, position.y, position.z];
        }
        public byte[,,] GetInputData()
        {
            return inputData;
        }

        public void SetPrefabs(List<PlyModelPrefab> prefabs)
        {
            prefabs = prefabs.ToList();
        }
        public List<PlyModelPrefab> GetPrefabs()
        {
            return prefabs;
        }
    }
}