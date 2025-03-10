using Game.Map.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Map.WFC
{
    public class WFCPresenter : MonoBehaviour
    {
        //public event Action<Vector3Int, PlyModel> OnModelUpdated;

        private WFCView view;
        private WFCModel model;

        private void Awake()
        {
            view = GetComponent<WFCView>();
            model = GetComponent<WFCModel>();
        }

        public void SetInput(Vector3Int position, byte value)
        {
            model.SetInput(position, value);
            RecalculateOutput(model.GetInputData(), model.GetPrefabs());
        }

        public byte GetInput(Vector3Int position)
        {
            return model.GetInput(position);
        }

        public void SetPlyModelPrefabs(List<PlyModelPrefab> prefabs)
        {
            model.SetPrefabs(prefabs);
        }

        /// <summary>
        /// Führt die Wave Function Collapse aus: Jede Zelle im 3D-Gitter besitzt anfänglich alle möglichen Tile-IDs.
        /// Feste Eingaben (inputData != 0) werden fixiert. Anschließend wird in einer Schleife der Zustand mit
        /// minimaler Entropie ausgewählt, kollabiert und mittels Constraint–Propagation die Umgebung aktualisiert.
        /// </summary>
        private void RecalculateOutput(byte[,,] inputData, List<PlyModelPrefab> prefabs)
        {
            int size = WFCModel.worldSize;
            byte[,,] outputData = new byte[size, size, size];

            // Erzeuge für jede Zelle eine Liste möglicher Tile-IDs.
            List<byte>[,,] possibilities = new List<byte>[size, size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        possibilities[x, y, z] = new List<byte>();
                        for (byte i = 0; i < prefabs.Count; i++) 
                        {
                            if (prefabs[i].allowedInputs.Contains(inputData[x, y, z]))
                            {
                                possibilities[x, y, z].Add(i);
                            }
                        }
                    }
                }
            }

            // Erzeuge eine Queue zur Constraint–Propagation.
            Queue<Vector3Int> propagationQueue = new Queue<Vector3Int>();
            // Alle fixierten Zellen (mit nur einer Möglichkeit) werden in die Queue aufgenommen.
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        if (possibilities[x, y, z].Count == 1)
                        {
                            propagationQueue.Enqueue(new Vector3Int(x, y, z));
                        }
                    }
                }
            }

            System.Random rnd = new System.Random(0);

            // Hauptschleife: solange es noch Zellen gibt, die nicht kollabiert sind.
            while (true)
            {
                bool allCollapsed = true;
                Vector3Int cellToCollapse = new Vector3Int();
                int minEntropy = int.MaxValue;

                // Finde die Zelle mit der geringsten Anzahl an Möglichkeiten (aber >1)
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        for (int z = 0; z < size; z++)
                        {
                            int count = possibilities[x, y, z].Count;
                            if (count > 1)
                            {
                                allCollapsed = false;
                                if (count < minEntropy)
                                {
                                    minEntropy = count;
                                    cellToCollapse = new Vector3Int(x, y, z);
                                }
                            }
                        }
                    }
                }

                // Sind alle Zellen kollabiert, so beenden wir die Schleife.
                if (allCollapsed)
                    break;

                // Kollabiere die ausgewählte Zelle: wähle zufällig einen möglichen Wert.
                List<byte> cellPoss = possibilities[cellToCollapse.x, cellToCollapse.y, cellToCollapse.z];
                byte chosen = cellPoss[rnd.Next(cellPoss.Count)];
                possibilities[cellToCollapse.x, cellToCollapse.y, cellToCollapse.z].Clear();
                possibilities[cellToCollapse.x, cellToCollapse.y, cellToCollapse.z].Add(chosen);
                propagationQueue.Enqueue(cellToCollapse);

                // Constraint–Propagation: Aktualisiere alle Nachbarzellen.
                while (propagationQueue.Count > 0)
                {
                    Vector3Int pos = propagationQueue.Dequeue();
                    // Da diese Zelle bereits kollabiert ist, enthält sie nur einen Wert:
                    byte collapsedValue = possibilities[pos.x, pos.y, pos.z][0];

                    foreach (var neighbor in GetNeighbors(pos, size))
                    {
                        List<byte> neighborPoss = possibilities[neighbor.x, neighbor.y, neighbor.z];
                        int beforeCount = neighborPoss.Count;
                        List<byte> allowed = new List<byte>();

                        // Für jede Möglichkeit im Nachbarn prüfen wir, ob sie kompatibel ist.
                        foreach (byte possibility in neighborPoss)
                        {
                            if (IsCompatible(collapsedValue, possibility))
                            {
                                allowed.Add(possibility);
                            }
                        }

                        if (allowed.Count < neighborPoss.Count)
                        {
                            possibilities[neighbor.x, neighbor.y, neighbor.z] = allowed;
                            // Falls ein Widerspruch entsteht (keine Möglichkeit mehr vorhanden), wird hier vereinfacht ein Reset durchgeführt.
                            if (allowed.Count == 0)
                            {
                                foreach (var prefab in prefabs)
                                {
                                    //allowed.Add(prefab.Value);
                                }
                                possibilities[neighbor.x, neighbor.y, neighbor.z] = allowed;
                            }
                            propagationQueue.Enqueue(neighbor);
                        }
                    }
                }
            }

            // Erstelle das finale OutputData-Array und informiere per Event über die Aktualisierung.
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        outputData[x, y, z] = possibilities[x, y, z][0];
                        //OnModelUpdated?.Invoke(new Vector3Int(x, y, z), new PlyModel(outputData[x, y, z]));
                    }
                }
            }

            // Optional: Das berechnete OutputData kann im Model gespeichert oder an die View weitergereicht werden.
            //model.OutputData = outputData;
        }

        /// <summary>
        /// Liefert alle gültigen Nachbarpositionen (6-Richtungen) einer gegebenen Position zurück.
        /// </summary>
        private IEnumerable<Vector3Int> GetNeighbors(Vector3Int pos, int size)
        {
            Vector3Int[] directions = new Vector3Int[]
            {
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(0, 0, 1),
                new Vector3Int(0, 0, -1)
            };

            foreach (var d in directions)
            {
                Vector3Int neighbor = pos + d;
                if (neighbor.x >= 0 && neighbor.x < size &&
                    neighbor.y >= 0 && neighbor.y < size &&
                    neighbor.z >= 0 && neighbor.z < size)
                {
                    yield return neighbor;
                }
            }
        }

        /// <summary>
        /// Prüft, ob zwei Tile-Werte kompatibel sind.
        /// In diesem Beispiel ist die Regel: Zwei Tiles sind kompatibel, wenn ihre Werte sich um höchstens 1 unterscheiden.
        /// </summary>
        private bool IsCompatible(byte collapsedValue, byte neighborValue)
        {
            return Math.Abs(collapsedValue - neighborValue) <= 1;
        }
    }
}
