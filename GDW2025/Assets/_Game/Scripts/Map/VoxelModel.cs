using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Map
{
    public class VoxelModel : MonoBehaviour
    {
        public event Action<Vector3Int> OnChunkUpdated;
        public const int chunkSize = 32; 
        
        private Dictionary<Vector3Int, int[,,]> data = new();
        private HashSet<Vector3Int> dirtyChunks = new();
        
        public void SetValue(Vector3Int position, int value, bool silent = false)
        {
            Vector3Int key = GetChunkKey(position);
            if (!data.ContainsKey(key))
            {
                data[key] = new int[chunkSize, chunkSize, chunkSize];
            }
            Vector3Int keyInChunk = GetKeyInChunk(position);
            data[key][keyInChunk.x, keyInChunk.y, keyInChunk.z] = value;

            if(!silent)
            {
                OnChunkUpdated?.Invoke(key);
            }
            else
            {
                dirtyChunks.Add(key);
            }
        }
        public void UpdateDirtyChunks()
        {
            foreach(Vector3Int chunk in dirtyChunks)
            {
                OnChunkUpdated?.Invoke(chunk);
            }
            dirtyChunks.Clear();
        }
        public int GetValue(Vector3Int position)
        {
            Vector3Int key = GetChunkKey(position);
            if (!data.ContainsKey(key))
            {
                return 0;
            }
            Vector3Int keyInChunk = GetKeyInChunk(position);
            return data[key][keyInChunk.x, keyInChunk.y, keyInChunk.z];
        }

        private Vector3Int GetChunkKey(Vector3Int position)
        {
            return new Vector3Int(Mathf.FloorToInt(position.x / (float)chunkSize) * chunkSize,
                Mathf.FloorToInt(position.y / (float)chunkSize) * chunkSize, 
                Mathf.FloorToInt(position.z / (float)chunkSize) * chunkSize);
        }
        private Vector3Int GetKeyInChunk(Vector3Int position)
        {
            return new Vector3Int(position.x % chunkSize, 
                position.y % chunkSize, 
                position.z % chunkSize);
        }
    }
}