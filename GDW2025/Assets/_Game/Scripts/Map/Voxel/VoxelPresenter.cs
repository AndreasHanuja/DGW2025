using DG.Tweening;
using Game.Map.Models;
using Game.Map.WFC;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Game.Map.Voxel
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

        public void SetValue(Vector3Int position, int value, bool silent = false)
        {
            model.SetValue(position, value, silent);
        }
        public void UpdateDirtyChunks()
        {
            model.UpdateDirtyChunks();
        }

        public void SetStructure(Vector3Int position, int[] values, bool clearAirAboveStructure = true)
        {
            Vector3Int chunkKeyTmp = position;
            int currentIndex = 0;
            while(currentIndex < values.Length)
            {
                model.ClearChunk(chunkKeyTmp);
                int[] chunkData = model.GetChunkData(chunkKeyTmp);
                int elementsToCopy = Mathf.Min(values.Length - currentIndex, chunkData.Length);
                Array.Copy(values, currentIndex, chunkData, 0, elementsToCopy);
                currentIndex += elementsToCopy;
                model.UpdateChunk(chunkKeyTmp);
                chunkKeyTmp.y += VoxelModel.chunkSize;
            }
            if (clearAirAboveStructure)
            {
                model.ClearChunk(chunkKeyTmp);
                model.UpdateChunk(chunkKeyTmp);
            }
        }
        public void AnimateStructure(Vector3Int position, int[,,] values, float duration)
        {
            List<(Vector3Int, int)> filteredValues = new();
            for(int x = 0; x < values.GetLength(0); x++)
            {
                for (int z = 0; z < values.GetLength(2); z++)
                {
                    for (int y = 0; y < values.GetLength(1); y++)
                    {
                        if (values[x,y,z] != 0)
                        {
                            filteredValues.Add((new Vector3Int(x, y, z), values[x, y, z]));
                        }
                    }
                }
            }

            int lastValue = 0;
            DOVirtual.Int(0, filteredValues.Count, duration, value =>
            {
                if (value > lastValue + 1)
                {
                    for (int i = lastValue + 1; i <= value; i++)
                    {
                        (Vector3Int, int) t = filteredValues[i];
                        model.SetValue(position + new Vector3Int(t.Item1.x, t.Item1.y, t.Item1.z), t.Item2, silent: true);
                    }
                    model.UpdateDirtyChunks();
                }
                lastValue = value;
            }).SetEase(Ease.InOutSine);
        }
        public int GetValue(Vector3Int position)
        {
            return model.GetValue(position);
        }


        public void GenerateGroundStructure(byte type, Vector2Int gridPosition)
        {
            int[] ints = new int[4096];
            int color = GroundToColor(type);

            for (int i = 0; i < 256; i++)
            {
                ints[3840 + i] = color;
            }
            SetStructure(new Vector3Int(gridPosition.x * 16, -16, gridPosition.y * 16), ints, false);
        }
        private int GroundToColor(byte ground)
        {
            switch (ground)
            {
                case 0:
                    return (64 << 24) + (178 << 16) + (64 << 8) + 255;
                case 1:
                    return (200 << 24) + (200 << 16) + (200 << 8) + 255;
                case 2:
                    return (190 << 24) + (50 << 16) + (220 << 8) + 255;
                case 3:
                    return (32 << 24) + (32 << 16) + (32 << 8) + 255;
            }
            return 0;
        }


        private void UpdateChunkHandler(Vector3Int chunkKey)
        {
            view.UpdateChunk(model.GetChunkData(chunkKey), chunkKey);
        }
    }
}