using DG.Tweening;
using Game.Map.Models;
using Game.Map.WFC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        private ConcurrentDictionary<int, int> workers = new ConcurrentDictionary<int, int>();

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
        private void Update()
        {
            if(workers.Count > 0)
            {
                UpdateDirtyChunks();
            }
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
        public async void AnimateStructure(Vector3Int position, int[,,] values, float duration)
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

            System.Random random = new System.Random();
            int workerID = random.Next();
            workers[workerID] = 0;
            while (filteredValues.Count > 0)
            {
                int index = random.Next(filteredValues.Count - 1);
                (Vector3Int, int) t = filteredValues[index];
                filteredValues.RemoveAt(index);
                model.SetValue(position + new Vector3Int(t.Item1.x, t.Item1.y, t.Item1.z), t.Item2, silent: true);
                if(filteredValues.Count % 4 == 0)
                {
                    await Task.Delay(1);
                }
            }
            UpdateDirtyChunks();
            workers.Remove(workerID, out int _);

        }
        public int GetValue(Vector3Int position)
        {
            return model.GetValue(position);
        }


        public void GenerateGroundStructure(byte type, Vector2Int gridPosition) 
        {
            int[,,] values = new int[16, 1, 16];
            if(type == 1)
            {
                byte[,] groundCache = WFCManager.Instance.GetGroundCache();
                bool northFree = gridPosition.y == 11 || groundCache[gridPosition.x, gridPosition.y + 1] != 1;
                bool eastFree = gridPosition.x == 11 || groundCache[gridPosition.x + 1, gridPosition.y] != 1;
                bool southFree = gridPosition.y == 0 || groundCache[gridPosition.x, gridPosition.y - 1] != 1;
                bool westFree = gridPosition.x == 0 || groundCache[gridPosition.x - 1, gridPosition.y] != 1;

                for (int x = 0; x < 16; x++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        values[x, 0, z] = GetSolarColor(northFree, eastFree, southFree, westFree, x, z);
                    }
                }
                byte[,] inputCache = WFCManager.Instance.GetInputCache();
                if (inputCache[gridPosition.x,gridPosition.y] == 0)
                {
                    model.ClearChunk(new Vector3Int(gridPosition.x * 16, 0, gridPosition.y * 16), true);
                }
                AnimateStructure(new Vector3Int(gridPosition.x * 16, -1, gridPosition.y * 16), values, 2);
                return;
            }
            int[] ints = new int[4096];
            int color = GroundToColor(type);

            for (int i = 0; i < 256; i++)
            {
                ints[3840 + i] = color;
            }
            SetStructure(new Vector3Int(gridPosition.x * 16, -16, gridPosition.y * 16), ints, false);
        }
        private int GetSolarColor(bool northFree, bool eastFree, bool southFree, bool westFree, int x, int y)
        {
            if (northFree && y == 15)
            {
                return (104 << 24) + (216 << 16) + (245 << 8) + 255; //blau
            }
            if (eastFree && x == 15)
            {
                return (104 << 24) + (216 << 16) + (245 << 8) + 255; //blau
            }
            if (southFree && y == 0)
            {
                return (104 << 24) + (216 << 16) + (245 << 8) + 255; //blau
            }
            if (westFree && x == 0)
            {
                return (104 << 24) + (216 << 16) + (245 << 8) + 255; //blau
            }
            if (northFree && y == 14)
            {
                return (197 << 24) + (200 << 16) + (189 << 8) + 255; //grau
            }
            if (eastFree && x == 14)
            {
                return (197 << 24) + (200 << 16) + (189 << 8) + 255; //grau
            }
            if (southFree && y == 1)
            {
                return (197 << 24) + (200 << 16) + (189 << 8) + 255; //grau
            }
            if (westFree && x == 1)
            {
                return (197 << 24) + (200 << 16) + (189 << 8) + 255; //grau
            }
            return (211 << 24) + (223 << 16) + (229 << 8) + 255; //weiß
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