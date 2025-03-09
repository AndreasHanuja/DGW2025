using System.Collections.Generic;
using UnityEngine;

namespace Game.Map
{
    public class VoxelModel : MonoBehaviour
    {
        private Dictionary<Vector3Int, int[,,]> data = new();
        public const int chunkSize = 16;

        public void SetValue(Vector3Int position, int value)
        {
            Vector3Int key = GetChunkKey(position);
            if (!data.ContainsKey(key))
            {
                data[key] = new byte[chunkSize, chunkSize, chunkSize];
            }
            Vector3Int keyInChunk = GetKeyInChunk(position);
            data[key][keyInChunk.x, keyInChunk.y, keyInChunk.z] = value;
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
            return new Vector3Int(position.x / chunkSize * chunkSize, position.y / chunkSize * chunkSize, position.z / chunkSize * chunkSize);
        }
        private Vector3Int GetKeyInChunk(Vector3Int position)
        {
            return new Vector3Int(position.x % chunkSize, position.y % chunkSize, position.z % chunkSize);
        }
    }
}