using UnityEngine;

namespace Game.Map
{

    [RequireComponent(typeof(VoxelModel)), RequireComponent(typeof(VoxelView))]
    public class VoxelPresenter : MonoBehaviour
    {
        private VoxelModel model;
        private VoxelView view;

        private void Start()
        {
            model = GetComponent<VoxelModel>();
            view = GetComponent<VoxelView>();
        }

        public void SetValue(Vector3Int position, byte value)
        {
            model.SetValue(position, value);
        }
        public byte GetValue(Vector3Int position)
        {
            return model.GetValue(position);
        }
    }
}