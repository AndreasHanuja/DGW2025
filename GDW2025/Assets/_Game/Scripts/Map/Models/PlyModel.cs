using UnityEngine;
namespace Game.Map.Models
{
    public class PlyModel
    {
        public PlyModelPrefab prefab;
        public Vector3Int offset;

        public PlyModel(Vector3Int offset, PlyModelPrefab prefab)
        {
            this.prefab = prefab;
            this.offset = offset;
        }
    }
}

