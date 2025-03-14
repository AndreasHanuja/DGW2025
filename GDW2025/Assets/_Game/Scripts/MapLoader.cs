using Game.Map.Voxel;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [SerializeField] private Vector3Int mapOffset;
    private void Start()
    {
        LoadMap();
    }
    private void LoadMap()
    {
        string fullPath = Path.Combine(Application.dataPath, ".Models/Map.ply");

        string[] fileData = { };

        if (File.Exists(fullPath))
        {
            fileData = File.ReadAllLines(fullPath);
            bool headerFinish = false;
            List<int4> unsortedValues = new List<int4>();
            foreach (string line in fileData)
            {
                if (headerFinish == true)
                {
                    string[] values = line.Split(' ');

                    int xCord = int.Parse(values[0]);
                    int yCord = int.Parse(values[2]);
                    int zCord = int.Parse(values[1]);
                    int r = int.Parse(values[3]);
                    int g = int.Parse(values[4]);
                    int b = int.Parse(values[5]);

                    int rgbValue = (r << 24) + (g << 16) + (b << 8) + 255;

                    VoxelPresenter.Instance.SetValue(new Vector3Int(xCord, yCord, zCord) + mapOffset, rgbValue, true);
                }

                if (line == "end_header")
                {
                    headerFinish = true;
                }
            }

            //todo init ground for WFC
            VoxelPresenter.Instance.UpdateDirtyChunks();
        }
    }
}
