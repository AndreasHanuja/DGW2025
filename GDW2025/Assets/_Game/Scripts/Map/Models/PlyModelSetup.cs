using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Game.Map.Models
{
    [CreateAssetMenu(fileName = "PlyModel", menuName = "ScriptableObjects/PlyModel", order = 1)]
    public class PlyModelSetup : ScriptableObject
    {
        [SerializeField] private string filepath;
        public List<PlyModelPrefab> content = new List<PlyModelPrefab>();

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
                Debug.Log($"Datei erfolgreich gelesen. Gr��e: {fileData.Length} Bytes");

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
                    content[4].data[normedZ, normedY, normedX] = block[3];

                    for (int i = 1; i < 4; i++)
                    {
                        int newX = content[0].data.GetLength(0) - 1 - normedZ;
                        int newZ = normedX;

                        normedX = newX;
                        normedZ = newZ;

                        content[i].data[normedX, normedY, normedZ] = block[3];
                        content[i+4].data[normedZ, normedY, normedX] = block[3];
                    }

                }

            }
            else
            {
                Debug.LogError($"Datei nicht gefunden: {fullPath}");
            }

            if (Application.isPlaying)
            {
                Voxel.VoxelPresenter.Instance.SetStructure(Vector3Int.zero, content[0].data);
                Voxel.VoxelPresenter.Instance.SetStructure(new Vector3Int(16, 0, 0), content[1].data);
                Voxel.VoxelPresenter.Instance.SetStructure(new Vector3Int(32, 0, 0), content[2].data);
                Voxel.VoxelPresenter.Instance.SetStructure(new Vector3Int(48, 0, 0), content[3].data);
                Voxel.VoxelPresenter.Instance.SetStructure(new Vector3Int(64, 0, 0), content[4].data);
                Voxel.VoxelPresenter.Instance.SetStructure(new Vector3Int(80, 0, 0), content[5].data);
                Voxel.VoxelPresenter.Instance.SetStructure(new Vector3Int(96, 0, 0), content[6].data);
                Voxel.VoxelPresenter.Instance.SetStructure(new Vector3Int(112, 0, 0), content[7].data);
            }
        }

        public void setHashs()
        {
            int[,] colorData = new int[content[0].data.GetLength(0), content[0].data.GetLength(0)];

            for (int modell = 0; modell < 8; modell++)
            {

                for (int z = 0; z < content[0].data.GetLength(0); z += content[0].data.GetLength(0) - 1)
                {
                    for (int x = 0; x < content[0].data.GetLength(0); x++)
                    {
                        for (int y = 0; y < content[0].data.GetLength(0); y++)
                        {
                            colorData[x, y] = content[modell].data[x, y, z];
                        }
                    }

                    content[modell].seitenHashs[z / 15] = ComputeSha256Hash(colorData);

                }

                for (int z = 0; z < content[0].data.GetLength(0); z += content[0].data.GetLength(0) - 1)
                {
                    for (int x = 0; x < content[0].data.GetLength(0); x++)
                    {
                        for (int y = 0; y < content[0].data.GetLength(0); y++)
                        {
                            colorData[x, y] = content[modell].data[x, z, y];
                        }
                    }

                    content[modell].seitenHashs[2 + (z / 15)] = ComputeSha256Hash(colorData);

                }

                for (int z = 0; z < content[0].data.GetLength(0); z += content[0].data.GetLength(0) - 1)
                {
                    for (int x = 0; x < content[0].data.GetLength(0); x++)
                    {
                        for (int y = 0; y < content[0].data.GetLength(0); y++)
                        {
                            colorData[x, y] = content[modell].data[z, x, y];
                        }
                    }

                    content[modell].seitenHashs[4 + (z / 15)] = ComputeSha256Hash(colorData);

                }
            } 
        }

        public static string ComputeSha256Hash(int[,] colorData)
        {
            int width = colorData.GetLength(0);
            int height = colorData.GetLength(1);

            // Byte-Array f�r die Farbwerte (Jeder int -> 4 Bytes)
            byte[] bytes = new byte[width * height * 4];
            Buffer.BlockCopy(colorData, 0, bytes, 0, bytes.Length);

            // SHA256-Hash berechnen
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

    }
}
