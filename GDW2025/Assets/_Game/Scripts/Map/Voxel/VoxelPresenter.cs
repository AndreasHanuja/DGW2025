using DG.Tweening;
using Game.Map.Models;
using Game.Map.WFC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        public async void AnimateStructure(Vector3Int position, int[] values, bool ordered, bool clearAbove)
        {
            Vector3Int chunkKey = VoxelModel.GetChunkKey(position);
            int positionIndex = VoxelModel.GetKeyInChunk(position - chunkKey);

            if (clearAbove)
            {
                model.ClearChunk(chunkKey + new Vector3Int(0, 16, 0), true);
            }

            if (values.Length + positionIndex <= 4096)
            {
                await AnimateSubStructure(position, values, ordered);
                return;
            }

            int part1 = 4096 - positionIndex;
            int[] part1Array = new int[part1];
            Array.Copy(values, 0, part1Array, 0, part1);
            await AnimateSubStructure(position, part1Array, ordered);

            int part2 = values.Length - part1;
            int[] part2Array = new int[part2];
            Array.Copy(values, part1, part2Array, 0, part2);

            await AnimateSubStructure(chunkKey + new Vector3Int(0, 16, 0), part2Array, ordered);
        }
        public async Task AnimateSubStructure(Vector3Int position, int[] values, bool ordered)
        {
            List<(int, int)> filteredValues = new();
            Vector3Int chunkKey = VoxelModel.GetChunkKey(position);
            int positionIndex = VoxelModel.GetKeyInChunk(position - chunkKey);
            int realCount = 0;
            for (int i = values.Length - 1; i >= 0; i--) 
            {
                if (values[i] != 0)
                {
                    realCount++;
                }
                filteredValues.Add((positionIndex + i, values[i]));
            }

            System.Random random = new System.Random();
            int workerID = random.Next();
            int index;
            workers[workerID] = 0;
            model.SetValue(position, 0, silent: true);

            int[] data = model.GetChunkData(chunkKey);

            int skip = 0;
            int skipCount = realCount / 25;
            while (filteredValues.Count > 0)
            {
                index = ordered ? filteredValues.Count - 1 : random.Next(filteredValues.Count - 1);
                (int, int) t = filteredValues[index];
                filteredValues.RemoveAt(index);
                data[t.Item1] = t.Item2;
                model.SetChunkDirty(chunkKey);

                if(t.Item2 != 0)
                {
                    skip++;
                }
                if(skip >= skipCount)
                {
                    await Task.Delay(1);
                    skip = 0;
                }
            }
            model.ClearChunk(chunkKey);
            Array.Copy(values, 0, model.GetChunkData(chunkKey), positionIndex, values.Length);
            model.UpdateChunk(chunkKey);
            workers.Remove(workerID, out int _);

        }
        public int GetValue(Vector3Int position)
        {
            return model.GetValue(position);
        }


        public void GenerateGroundStructure(byte type, Vector2Int gridPosition) 
        {
            System.Random random = new System.Random();

            int[] values = new int[256];
            byte[,] groundCache = WFCManager.Instance.GetGroundCache();

            bool northFree = gridPosition.y == 11 || groundCache[gridPosition.x, gridPosition.y + 1] != type;
            bool eastFree = gridPosition.x == 11 || groundCache[gridPosition.x + 1, gridPosition.y] != type;
            bool southFree = gridPosition.y == 0 || groundCache[gridPosition.x, gridPosition.y - 1] != type;
            bool westFree = gridPosition.x == 0 || groundCache[gridPosition.x - 1, gridPosition.y] != type;

            for (int i = 0; i < 256; i++)
            {
                switch (type)
                {
                    case 1:
                        values[i] = GetSolarColor(northFree, eastFree, southFree, westFree, i % 16, i / 16);
                        break;
                    case 2:
                        values[i] = GetFantasyColor(northFree, eastFree, southFree, westFree, i % 16, i / 16, random);
                        break;
                }
            }

            byte[,] inputCache = WFCManager.Instance.GetInputCache();
            if (inputCache[gridPosition.x, gridPosition.y] == 0)
            {
                model.ClearChunk(new Vector3Int(gridPosition.x * 16, 0, gridPosition.y * 16), true);
            }

            AnimateStructure(new Vector3Int(gridPosition.x * 16, -1, gridPosition.y * 16), values, type == 1, false);
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
        private int GetFantasyColor(bool northFree, bool eastFree, bool southFree, bool westFree, int x, int y, System.Random random)
        {
            if (northFree && y > 12)
            {
                return random.Next(100) < 60 + (y - 15) * 10 ? 
                    (173 << 24) + (129 << 16) + (178 << 8) + 255 :
                    (114 << 24) + (161 << 16) + (82 << 8) + 255; 
            }
            if (eastFree && x > 12)
            {
                return random.Next(100) < 60 + (x - 15) * 10 ?
                   (173 << 24) + (129 << 16) + (178 << 8) + 255 :
                    (114 << 24) + (161 << 16) + (82 << 8) + 255;
            }
            if (southFree && y < 3)
            {
                return random.Next(100) < 60 + y * 10 ?
                   (173 << 24) + (129 << 16) + (178 << 8) + 255 :
                    (114 << 24) + (161 << 16) + (82 << 8) + 255;
            }
            if (westFree && x < 3)
            {
                return random.Next(100) < 60 + x * 10 ?
                   (173 << 24) + (129 << 16) + (178 << 8) + 255 :
                    (114 << 24) + (161 << 16) + (82 << 8) + 255;
            }
            return (173 << 24) + (129 << 16) + (178 << 8) + 255;
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