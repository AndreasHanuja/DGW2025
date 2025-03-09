using System.Collections.Generic;
using UnityEngine;

namespace Game.Map
{
    public class VoxelModel : MonoBehaviour
    {
        private Dictionary<Vector3Int, byte[,,]> data = new();
        private const int chunkSize = 16;

        public void SetValue(Vector3Int position, byte value)
        {
            Vector3Int key = GetChunkKey(position);
            if (!data.ContainsKey(key))
            {
                data[key] = new byte[chunkSize, chunkSize, chunkSize];
            }

            //data 
        }
        public byte GetValue(Vector3Int position)
        {
            return 0;
        }

        private Vector3Int GetChunkKey(Vector3Int position)
        {
            return new Vector3Int(position.x / chunkSize * chunkSize, position.y / chunkSize * chunkSize, position.z / chunkSize * chunkSize);
        }
    }
}