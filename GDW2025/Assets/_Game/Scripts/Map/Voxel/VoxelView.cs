using System.Collections.Generic;
using UnityEngine;

namespace Game.Map.Voxel
{
    public class VoxelView : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float randomColorRange = 0.1f;
        [SerializeField] private GameObject chunkPrefab;

        private Dictionary<Vector3Int, MeshFilter> chunks = new();

        public void UpdateChunk(VoxelModel model, Vector3Int chunkKey)
        {
            if (!chunks.ContainsKey(chunkKey))
            {
                chunks[chunkKey] = Instantiate(chunkPrefab, transform).GetComponent<MeshFilter>();
            }

            MeshFilter meshFilter = chunks[chunkKey];
            BuildChunk(meshFilter, model, chunkKey);
        }
      
        private void BuildChunk(MeshFilter meshFilter, VoxelModel model, Vector3Int chunkKey)
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();
            List<Color> colors = new();

            for(int x = chunkKey.x; x < chunkKey.x + VoxelModel.chunkSize; x++)
            {
                for (int y = chunkKey.y; y < chunkKey.y + VoxelModel.chunkSize; y++)
                {
                    for (int z = chunkKey.z; z < chunkKey.z + VoxelModel.chunkSize; z++)
                    {
                        Vector3Int position = new Vector3Int(x, y, z);
                        uint chunkValue = (uint) model.GetValue(position);

                        if(chunkValue == 0)
                        {
                            continue;
                        }

                        int vertexCount = vertices.Count;

                        if(model.GetValue(position + Vector3Int.back) == 0)
                        {
                            vertices.Add(position);
                            vertices.Add(position + Vector3.up);
                            vertices.Add(position + Vector3.up + Vector3.right);
                            vertices.Add(position + Vector3.right);
                        }

                        if (model.GetValue(position + Vector3Int.up) == 0)
                        {
                            vertices.Add(position + Vector3.up);
                            vertices.Add(position + Vector3.up + Vector3.forward);
                            vertices.Add(position + Vector3.up + Vector3.forward + Vector3.right);
                            vertices.Add(position + Vector3.up + Vector3.right);
                        }

                        if (model.GetValue(position + Vector3Int.forward) == 0)
                        {
                            vertices.Add(position + Vector3.forward + Vector3.right);
                            vertices.Add(position + Vector3.forward + Vector3.right + Vector3.up);
                            vertices.Add(position + Vector3.forward + Vector3.up);
                            vertices.Add(position + Vector3.forward);
                        }

                        if (model.GetValue(position + Vector3Int.down) == 0)
                        {
                            vertices.Add(position + Vector3.forward);
                            vertices.Add(position);
                            vertices.Add(position + Vector3.right);
                            vertices.Add(position + Vector3.forward + Vector3.right);
                        }

                        if (model.GetValue(position + Vector3Int.left) == 0)
                        {
                            vertices.Add(position + Vector3.forward);
                            vertices.Add(position + Vector3.forward + Vector3.up);
                            vertices.Add(position + Vector3.up);
                            vertices.Add(position);
                        }

                        if (model.GetValue(position + Vector3Int.right) == 0)
                        {
                            vertices.Add(position + Vector3.right);
                            vertices.Add(position + Vector3.right + Vector3.up);
                            vertices.Add(position + Vector3.right + Vector3.forward + Vector3.up);
                            vertices.Add(position + Vector3.right + Vector3.forward);
                        }

                        int addedVertices = vertices.Count - vertexCount;
                        Color voxelColor = new Color(
                             (chunkValue >> 24) / 255f,
                             ((chunkValue >> 16) & 255) / 255f,
                             ((chunkValue >> 8) & 255) / 255f,
                             (chunkValue & 255) / 255f
                        );
                        Color.RGBToHSV(voxelColor, out float h, out float s, out float v);
                        System.Random random = new System.Random(position.GetHashCode());
                        v = Mathf.Clamp01(random.Next((int)((v - randomColorRange) * 100), (int)((v + randomColorRange) * 100)) / 100f);
                        voxelColor = Color.HSVToRGB(h, s, v);

                        for(int i = 0; i<addedVertices; i++)
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

            meshFilter.mesh.SetVertices(vertices);
            meshFilter.mesh.SetTriangles(triangles, 0);
            meshFilter.mesh.SetColors(colors);
            meshFilter.mesh.RecalculateNormals();
        }
    }
}