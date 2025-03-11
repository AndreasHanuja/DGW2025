using UnityEngine;
namespace Game.Map.Models
{
    public class PlyModel
    {
        public readonly Vector3Int offset;
        public int[] data { get => prefab.data; }

        private PlyModelPrefab prefab;

        public PlyModel(Vector3Int offset, PlyModelPrefab prefab)
        {
            this.prefab = prefab;
            this.offset = offset;
        }
    }
}

