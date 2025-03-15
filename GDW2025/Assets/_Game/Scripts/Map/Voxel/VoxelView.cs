using Game.Map.WFC;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;
// using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace Game.Map.Voxel
{
    public class VoxelView : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float randomColorRange = 0.1f;
        [SerializeField] private GameObject chunkPrefab;

        private Dictionary<Vector3Int, MeshFilter> chunks = new();

        public void UpdateChunk(int[] chunkData, Vector3Int chunkKey)
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();
            List<Color> colors = new();
            BuildChunk(vertices, triangles, colors, chunkData, chunkKey);

            MainThreadDispatcher.Enqueue(() => {
                if (!chunks.ContainsKey(chunkKey))
                {
                    chunks[chunkKey] = Instantiate(chunkPrefab, transform).GetComponent<MeshFilter>();
                    chunks[chunkKey].transform.position = chunkKey;
                }
                MeshFilter meshFilter = chunks[chunkKey];
                meshFilter.mesh = new Mesh();
                meshFilter.mesh.SetVertices(vertices);
                meshFilter.mesh.SetTriangles(triangles, 0);
                meshFilter.mesh.SetColors(colors);
                meshFilter.mesh.RecalculateNormals();
            });
        }

        private void BuildChunk(List<Vector3> vertices, List<int> triangles, List<Color> colors, int[] chunkData, Vector3Int chunkKey)
        {
            for(int x = 0; x < VoxelModel.chunkSize; x++)
            {
                for (int y = 0; y < VoxelModel.chunkSize; y++)
                {
                    for (int z = 0; z < VoxelModel.chunkSize; z++)
                    {
                        Vector3Int position = new Vector3Int(x, y, z);                       

                        if(IsEmpty(chunkData, position))
                        {
                            continue;
                        }

                        int vertexCount = vertices.Count;

                        if(IsEmpty(chunkData, position + Vector3Int.back))
                        {
                            vertices.Add(position);
                            vertices.Add(position + Vector3.up);
                            vertices.Add(position + Vector3.up + Vector3.right);
                            vertices.Add(position + Vector3.right);
                        }

                        if (IsEmpty(chunkData, position + Vector3Int.up))
                        {
                            vertices.Add(position + Vector3.up);
                            vertices.Add(position + Vector3.up + Vector3.forward);
                            vertices.Add(position + Vector3.up + Vector3.forward + Vector3.right);
                            vertices.Add(position + Vector3.up + Vector3.right);
                        }

                        if (IsEmpty(chunkData, position + Vector3Int.forward))
                        {
                            vertices.Add(position + Vector3.forward + Vector3.right);
                            vertices.Add(position + Vector3.forward + Vector3.right + Vector3.up);
                            vertices.Add(position + Vector3.forward + Vector3.up);
                            vertices.Add(position + Vector3.forward);
                        }

                        if (IsEmpty(chunkData, position + Vector3Int.down))
                        {
                            vertices.Add(position + Vector3.forward);
                            vertices.Add(position);
                            vertices.Add(position + Vector3.right);
                            vertices.Add(position + Vector3.forward + Vector3.right);
                        }

                        if (IsEmpty(chunkData, position + Vector3Int.left))
                        {
                            vertices.Add(position + Vector3.forward);
                            vertices.Add(position + Vector3.forward + Vector3.up);
                            vertices.Add(position + Vector3.up);
                            vertices.Add(position);
                        }

                        if (IsEmpty(chunkData, position + Vector3Int.right))
                        {
                            vertices.Add(position + Vector3.right);
                            vertices.Add(position + Vector3.right + Vector3.up);
                            vertices.Add(position + Vector3.right + Vector3.forward + Vector3.up);
                            vertices.Add(position + Vector3.right + Vector3.forward);
                        }

                        int addedVertices = vertices.Count - vertexCount;

                        int dataRaw = chunkData[position.x +
                           position.z * VoxelModel.chunkSize +
                           position.y * VoxelModel.chunkSize * VoxelModel.chunkSize];
                        uint chunkValue = (uint)dataRaw;
                        Color voxelColor = new Color(
                             (chunkValue >> 24) / 255f,
                             ((chunkValue >> 16) & 255) / 255f,
                             ((chunkValue >> 8) & 255) / 255f,
                             (chunkValue & 255) / 255f
                        );
                        float originalAlpha = voxelColor.a;
                        Color.RGBToHSV(voxelColor, out float h, out float s, out float v);
                        Vector3Int pos = chunkKey + position;
                        System.Random random = new System.Random(GetCombinedHash(pos.x, pos.y, pos.z));
                        v = Mathf.Clamp01(random.Next((int)((v - randomColorRange) * 100), (int)((v + randomColorRange) * 100)) / 100f);
                        voxelColor = Color.HSVToRGB(h, s, v);

                        switch (dataRaw)
                        {
                            case ((255<<24) +(255<<16) + (0<<8) + 255): //fackel
                                voxelColor.a = 0.5f;
                                break;
                            case ((236 << 24) + (245 << 16) + (246 << 8) + 255): //Pilz blau
                                voxelColor.r = 0f;
                                voxelColor.g = 0f;
                                voxelColor.b = 1;
                                voxelColor.a = 0.5f;
                                break;
                            case ((246 << 24) + (236 << 16) + (244 << 8) + 255): //Pilz rot
                                voxelColor.r = 1f;
                                voxelColor.g = 0f;
                                voxelColor.b = 0.5f;
                                voxelColor.a = 0.5f;
                                break;
                            case ((255 << 24) + (123 << 16) + (0 << 8) + 255): //lava
                                voxelColor.a = 1f;
                                break;
                            case ((157 << 24) + (255 << 16) + (76 << 8) + 255): //Solarpunk 
                                voxelColor.r = 0.1f;
                                voxelColor.g = 1f;
                                voxelColor.b = 0.2f;
                                voxelColor.a = 1f;
                                break;
                            default:
                                voxelColor.a = 0;
                                break;
                        }
                        for (int i = 0; i<addedVertices; i++)
                        {
                            colors.Add(voxelColor);
                        }
                    }
                }
            }

            for (int i = 0; i < vertices.Count; i += 4)
            {
                triangles.Add(i);
                triangles.Add(i + 1);
                triangles.Add(i + 2);
                triangles.Add(i);
                triangles.Add(i + 2);
                triangles.Add(i + 3);
            }
        }

        private bool IsEmpty(int[] values, Vector3Int p)
        {
            return p.x < 0 || p.x >= VoxelModel.chunkSize ||
                 p.y < 0 || p.y >= VoxelModel.chunkSize ||
                 p.z < 0 || p.z >= VoxelModel.chunkSize ||
                 values[p.x +
                    p.z * VoxelModel.chunkSize +
                    p.y * VoxelModel.chunkSize * VoxelModel.chunkSize] == 0;
        }

        public static int GetCombinedHash(int x, int y, int z)
        {
            unchecked
            {
                // Startwert
                int hash = 17;
                // Kombiniere die Werte mit Primzahl‑Multiplikation
                hash = hash * 31 + x;
                hash = hash * 31 + y;
                hash = hash * 31 + z;

                // Abschließende Bit‑Mischung für eine bessere Verteilung
                hash ^= (hash >> 15);
                hash *= (int)0x85ebca6b;
                hash ^= (hash >> 13);
                hash *= (int)0xc2b2ae35;
                hash ^= (hash >> 16);

                return hash;
            }
        }
    }
}