using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Map
{
    public class VoxelView : MonoBehaviour
    {
        [SerializeField] private GameObject chunkPrefab;

        private List<MeshFilter> inactiveChunks = new();
        private Dictionary<Vector3Int, MeshFilter> activeChunks;

        public void RenderChunk(VoxelModel model, Vector3Int chunkKey)
        {
            if (!activeChunks.ContainsKey(chunkKey))
            {
                AddChunk(chunkKey);
            }

            MeshFilter meshFilter = activeChunks[chunkKey];
            BuildChunk(meshFilter, model, chunkKey);
        }

        private void AddChunk(Vector3Int chunkKey)
        {
            if (!inactiveChunks.Any())
            {
                inactiveChunks.Add(Instantiate(chunkPrefab, transform).GetComponent<MeshFilter>());
            }
            MeshFilter chunk = inactiveChunks[0];
            chunk.gameObject.SetActive(true);
            activeChunks[chunkKey] = chunk;
            inactiveChunks.RemoveAt(0);
        }

        private void BuildChunk(MeshFilter meshFilter, VoxelModel model, Vector3Int chunkKey)
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();
            List<Color> colors = new();

            for(int x = chunkKey.x; x < chunkKey.x + VoxelModel.chunkSize; x++)
            {
                for (int y = chunkKey.x; y < chunkKey.y + VoxelModel.chunkSize; y++)
                {
                    for (int z = chunkKey.x; z < chunkKey.z + VoxelModel.chunkSize; z++)
                    {
                        Vector3Int offset = new Vector3Int(x, y, z);
                        int chunkValue = model.GetValue(chunkKey + offset);

                        if(chunkValue == 0)
                        {
                            continue;
                        }

                        if(model.GetValue(chunkKey + offset + Vector3Int.back) != 0)
                        {
                            vertices.Add(offset);
                            vertices.Add(offset + Vector3.up);
                            vertices.Add(offset + Vector3.up + Vector3.right);
                            vertices.Add(offset + Vector3.right);
                        }

                        if (model.GetValue(chunkKey + offset + Vector3Int.up) != 0)
                        {
                            vertices.Add(offset + Vector3.up);
                            vertices.Add(offset + Vector3.up + Vector3.forward);
                            vertices.Add(offset + Vector3.up + Vector3.forward + Vector3.right);
                            vertices.Add(offset + Vector3.up + Vector3.right);
                        }

                        if (model.GetValue(chunkKey + offset + Vector3Int.forward) != 0)
                        {
                            vertices.Add(offset + Vector3.forward + Vector3.right);
                            vertices.Add(offset + Vector3.forward + Vector3.right + Vector3.up);
                            vertices.Add(offset + Vector3.forward + Vector3.up);
                            vertices.Add(offset + Vector3.forward);
                        }

                        if (model.GetValue(chunkKey + offset + Vector3Int.down) != 0)
                        {
                            vertices.Add(offset + Vector3.forward);
                            vertices.Add(offset);
                            vertices.Add(offset + Vector3.right);
                            vertices.Add(offset + Vector3.forward + Vector3.right);
                        }

                        if (model.GetValue(chunkKey + offset + Vector3Int.left) != 0)
                        {
                            /*
                            vertices.Add(offset + Vector3.forward);
                            vertices.Add(offset + Vector3.forward + Vector3.up);
                            vertices.Add(offset + Vector3.right);
                            vertices.Add(offset + Vector3.forward + Vector3.right);
                            */
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
        }
    }
}