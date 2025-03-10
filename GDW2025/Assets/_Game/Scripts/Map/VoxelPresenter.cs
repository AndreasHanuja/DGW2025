using UnityEngine;

namespace Game.Map
{
    [RequireComponent(typeof(VoxelModel)), RequireComponent(typeof(VoxelView))]
    public class VoxelPresenter : MonoBehaviour
    {
        public static VoxelPresenter Instance;

        private VoxelModel model;
        private VoxelView view;

        private void Awake()
        {
            model = GetComponent<VoxelModel>();
            view = GetComponent<VoxelView>();

            Instance = this;
        }

        private void OnEnable()
        {
            model.OnChunkUpdated += UpdateChunkHandler;
        }
        private void OnDisable()
        {
            model.OnChunkUpdated -= UpdateChunkHandler;
        }

        public void SetValue(Vector3Int position, int value)
        {
            model.SetValue(position, value);
        }
        public void SetStructure(Vector3Int position, int[,,] values)
        {
            for(int x = 0; x < values.GetLength(0); x++)
            {
                for (int y = 0; y < values.GetLength(1); y++)
                {
                    for (int z = 0; z < values.GetLength(2); z++)
                    {
                        model.SetValue(position + new Vector3Int(x, y, z), values[x, y, z], silent: true);                       
                    }
                }
            }
            model.UpdateDirtyChunks();            
        }
        public int GetValue(Vector3Int position)
        {
            return model.GetValue(position);
        }

        private void UpdateChunkHandler(Vector3Int chunkKey)
        {
            view.UpdateChunk(model, chunkKey);
        }
    }
}