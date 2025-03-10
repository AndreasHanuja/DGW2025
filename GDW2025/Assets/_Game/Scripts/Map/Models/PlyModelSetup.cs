using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Map.Models
{
    [CreateAssetMenu(fileName = "PlyModel", menuName = "ScriptableObjects/PlyModel", order = 1)]
    public class PlyModelSetup : ScriptableObject
    {
        [SerializeField] private string filepath;
        private List<PlyModelPrefab> content = new List<PlyModelPrefab>();

        public void LoadModel()
        {
            for (int i = 0; i < 8; i++)
            {
                content.Add(new PlyModelPrefab());
            }

            string fullPath = Path.Combine(Application.dataPath, filepath);
            string[] fileData = { };

            if (File.Exists(fullPath))
            {
                fileData = File.ReadAllLines(fullPath);
                Debug.Log($"Datei erfolgreich gelesen. Größe: {fileData.Length} Bytes");

                bool headerFinish = false;

                List<int[]> unsortedValues = new List<int[]>();

                foreach (string line in fileData)
                {

                    if (headerFinish == true)
                    {
                        string[] werte = line.Split(' ');

                        int xCord = int.Parse(werte[0]);
                        int yCord = int.Parse(werte[2]);
                        int zCord = int.Parse(werte[1]);
                        int r = int.Parse(werte[3]);
                        int g = int.Parse(werte[4]);
                        int b = int.Parse(werte[5]);

                        int rgbValue = (r << 24) + (g << 16) + (b << 8) + 255;

                        unsortedValues.Add(new int[4]{xCord,yCord,zCord,rgbValue});

                    }

                    if (line == "end_header")
                    {
                        headerFinish = true;
                    }

                }


                int minX = unsortedValues[0][0];
                int minY = unsortedValues[0][1];
                int minZ = unsortedValues[0][2];

                foreach (int[] block in unsortedValues)
                {
                    if (block[0] < minX)
                    {
                        minX = block[0];
                    }

                    if (block[1] < minY)
                    {
                        minY = block[1];
                    }

                    if (block[2] < minZ)
                    {
                        minZ = block[2];
                    }
                }

                foreach (int[] block in unsortedValues)
                {
                    int normedX = block[0] - minX;
                    int normedY = block[1] - minY;
                    int normedZ = block[2] - minZ;

                    content[0].data[normedX, normedY, normedZ] = block[3];

                    for (int i = 1; i < 4; i++)
                    {
                        int newX = content[0].data.GetLength(0) - 1 - normedZ;
                        int newZ = normedX;

                        normedX = newX;
                        normedZ = newZ;

                        content[i].data[normedX, normedY, normedZ] = block[3];
                    }

                }

            }
            else
            {
                Debug.LogError($"Datei nicht gefunden: {fullPath}");
            }

            if (Application.isPlaying)
            {
                VoxelPresenter.Instance.SetStructure(Vector3Int.zero, content[0].data);
                VoxelPresenter.Instance.SetStructure(new Vector3Int(16, 0, 0), content[1].data);
                VoxelPresenter.Instance.SetStructure(new Vector3Int(32, 0, 0), content[2].data);
                VoxelPresenter.Instance.SetStructure(new Vector3Int(48, 0, 0), content[3].data);
            }
        }
    }
}
