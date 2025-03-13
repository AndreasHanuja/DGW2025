using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Map.Models
{
    [CreateAssetMenu(fileName = "PlyModel", menuName = "ScriptableObjects/PlyModel", order = 1)]
    public class PlyModelSetup : ScriptableObject
    {
        [SerializeField] private List<int> allowedInputs;
        [SerializeField] private List<int> allowedGround;

        [SerializeField] private float weight;

		[SerializeField] private int pointValue = 0;
        public int PointValue => pointValue;

		public List<PlyModelPrefab> createdPrefabs = new List<PlyModelPrefab>();

        public void LoadModel()
        {
            createdPrefabs.Clear();

            for (int i = 0; i < 4; i++)
            {
                PlyModelPrefab prefab = new PlyModelPrefab();
                prefab.allowedInputs = allowedInputs.ToList();
                prefab.allowedGround = allowedGround.ToList();
                prefab.weight = weight;
                createdPrefabs.Add(prefab);                
            }

            string fullPath = Path.Combine(Application.dataPath, ".Models/"+ name.Replace("-", "/") + ".ply");
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

                        unsortedValues.Add(new int4(xCord, yCord, zCord, rgbValue));
                    }

                    if (line == "end_header")
                    {
                        headerFinish = true;
                    }
                }
                int minX = -8;
                int minY = unsortedValues.Min(v => v.y);
                int maxY = unsortedValues.Max(v => v.y);
                int minZ = -8;

                for(int i= 0; i < createdPrefabs.Count; i++)
                {
                    createdPrefabs[i].InitHeight(maxY - minY + 1);
                }

                foreach (int4 block in unsortedValues)
                {
                    int x = block.x - minX;
                    int y = block.y - minY;
                    int z = block.z - minZ;

                    int maxSize = PlyModelPrefab.modelSize - 1;
                    createdPrefabs[0].SetData(x, y, z, block.w);                                        //normal
                    createdPrefabs[1].SetData(z, y, maxSize - x, block.w);                              //normal rotiert 90°
                    createdPrefabs[2].SetData(maxSize - x, y, maxSize - z, block.w);                    //normal rotiert 180°
                    createdPrefabs[3].SetData(maxSize - z, y, x, block.w);                              //normal rotiert 270°

                    /*
                    createdPrefabs[4].SetData(maxSize - x, y, z, block.w);                              //gespiegelt
                    createdPrefabs[5].SetData(maxSize - z, y, maxSize - x, block.w);                    //gespiegelt rotiert 90°
                    createdPrefabs[6].SetData(x, y, maxSize - z, block.w);                              //gespiegelt rotiert 180°
                    createdPrefabs[7].SetData(z, y, x, block.w);                                        //gespiegelt rotiert 270°
                    */
                }                
            }

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }

        public void SetHashs()
        {
            for (int i = 0; i < createdPrefabs.Count; i++)
            {
                List<int> data = new List<int>();

                //north
                for (int x = 0; x < PlyModelPrefab.modelSize; x++)
                {
                    for (int y = 0; y < createdPrefabs[i].height; y++)
                    {
                        int value = createdPrefabs[i].GetData(x, y, PlyModelPrefab.modelSize - 1);
                        if(value == 0)
                        {
                            continue;
                        }
                        data.Add(x);
                        data.Add(y);
                        data.Add(value);
                    }
                }
                createdPrefabs[i].hashes[0] = ComputeSha256Hash(data);                
                data.Clear();

                //east
                for (int z = 0; z < PlyModelPrefab.modelSize; z++)
                {
                    for (int y = 0; y < createdPrefabs[i].height; y++)
                    {
                        int value = createdPrefabs[i].GetData(PlyModelPrefab.modelSize - 1, y, z);
                        if (value == 0)
                        {
                            continue;
                        }
                        data.Add(z);
                        data.Add(y);
                        data.Add(value);
                    }
                }
                createdPrefabs[i].hashes[1] = ComputeSha256Hash(data);
                data.Clear();

                //south
                for (int x = 0; x < PlyModelPrefab.modelSize; x++)
                {
                    for (int y = 0; y < createdPrefabs[i].height; y++)
                    {
                        int value = createdPrefabs[i].GetData(x, y, 0);
                        if (value == 0)
                        {
                            continue;
                        }
                        data.Add(x);
                        data.Add(y);
                        data.Add(value);
                    }
                }
                createdPrefabs[i].hashes[2] = ComputeSha256Hash(data);
                data.Clear();

                //west
                for (int z = 0; z < PlyModelPrefab.modelSize; z++)
                {
                    for (int y = 0; y < createdPrefabs[i].height; y++)
                    {
                        int value = createdPrefabs[i].GetData(0, y, z);
                        if (value == 0)
                        {
                            continue;
                        }
                        data.Add(z);
                        data.Add(y);
                        data.Add(value);
                    }
                }
                createdPrefabs[i].hashes[3] = ComputeSha256Hash(data);
            }

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif    
        }

        public static string ComputeSha256Hash(List<int> data)
        {
            byte [] bytes = data.SelectMany(i => BitConverter.GetBytes(i)).ToArray();

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

    }
}
