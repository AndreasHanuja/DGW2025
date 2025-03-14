using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Splines;

namespace Game.Map.Voxel
{
    public class VoxelModel : MonoBehaviour
    {
        public event Action<Vector3Int> OnChunkUpdated;
        public const int chunkSize = 16; 
        
        private ConcurrentDictionary<Vector3Int, int[]> data = new();
        private HashSet<Vector3Int> dirtyChunks = new();
        
        public void SetValue(Vector3Int position, int value, bool silent = false)
        {
            lock (dirtyChunks)
            {
                Vector3Int key = GetChunkKey(position);
                if (!data.ContainsKey(key))
                {
                    data[key] = new int[chunkSize * chunkSize * chunkSize];
                }
                int keyInChunk = GetKeyInChunk(position - key);
                data[key][keyInChunk] = value;

                if (!silent)
                {
                    OnChunkUpdated?.Invoke(key);
                }
                else
                {
                    dirtyChunks.Add(key);
                }
            }
        }
        public void ClearChunk(Vector3Int chunkKey, bool setDirty = false)
        {
            data.TryRemove(chunkKey, out int[] values);
            if(setDirty)
            {
                dirtyChunks.Add(chunkKey);
            }
        }
        public int[] GetChunkData(Vector3Int chunkKey)
        {
            if (!data.ContainsKey(chunkKey))
            {
                data[chunkKey] = new int[chunkSize * chunkSize * chunkSize];
            }
            return data[chunkKey];
        }
        public void UpdateDirtyChunks()
        {
            lock (dirtyChunks)
            {
                Parallel.ForEach(dirtyChunks, c => OnChunkUpdated?.Invoke(c));
                dirtyChunks.Clear();
            }
        }
        public void UpdateChunk(Vector3Int chunkKey)
        {
            OnChunkUpdated?.Invoke(chunkKey);
        }
        public int GetValue(Vector3Int position)
        {
            Vector3Int key = GetChunkKey(position);
            if (!data.ContainsKey(key))
            {
                return 0;
            }
            int keyInChunk = GetKeyInChunk(position);
            return data[key][keyInChunk];
        }

        private Vector3Int GetChunkKey(Vector3Int position)
        {
            return new Vector3Int(Mathf.FloorToInt(position.x / (float)chunkSize) * chunkSize,
                Mathf.FloorToInt(position.y / (float)chunkSize) * chunkSize, 
                Mathf.FloorToInt(position.z / (float)chunkSize) * chunkSize);
        }
        private int GetKeyInChunk(Vector3Int position)
        {
            return (position.x % chunkSize) + (position.z % chunkSize) * chunkSize + (position.y % chunkSize) * chunkSize * chunkSize;
        }
    }
}