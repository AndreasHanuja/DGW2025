
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Map.Models
{
    [Serializable]
    public class PlyModelPrefab
    {
        public const int modelSize = 16;

        [SerializeField, HideInInspector] public int height;

         public int[] data;
        public PlyModelSetup setup;
        [HideInInspector] public List<int> allowedInputs = new List<int>();
        [HideInInspector] public List<int> allowedGround = new List<int>();
        public float weight;
        [HideInInspector] public int id;

        public string[] hashes = new string[4]; // north, east, south, west

        public void InitHeight(int height)
        {
            this.height = height;
            data = new int[modelSize * modelSize * height];
        }
        public int GetData(int x, int y, int z)
        {
            return data[x + z * modelSize + y * modelSize * modelSize];
        }
        public void SetData(int x, int y, int z, int value)
        {
            data[x + z * modelSize + y * modelSize * modelSize] = value;
        }

        public PlyModel Instantiate(Vector3Int offset)
        {
            return new PlyModel(offset, this);
        }
        public bool CheckConnectivity(PlyModelPrefab other, Vector2Int direction)
        {
            if(direction == Vector2Int.up)
            {
                return hashes[0] == other.hashes[2];
            }
            if (direction == Vector2Int.right)
            {
                return hashes[1] == other.hashes[3];
            }
            if (direction == Vector2Int.down)
            {
                return hashes[2] == other.hashes[0];
            }
            if (direction == Vector2Int.left)
            {
                return hashes[3] == other.hashes[1];
            }
            return false;
        }        
    }


}