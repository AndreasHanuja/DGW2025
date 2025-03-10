using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Map
{
    [CreateAssetMenu(fileName = "PlyModel", menuName = "ScriptableObjects/PlyModel", order = 1)]
    public class PlyModel : ScriptableObject
    {
        [SerializeField] private string filepath;
        private List<int[,,]> content = new List<int[,,]>();

        public void LoadModel()
        {
            content.Add(new int[16, 16, 16]);
            string fullPath = Path.Combine(Application.dataPath, filepath);
            string[] fileData = { };

            if (File.Exists(fullPath))
            {
                fileData = File.ReadAllLines(fullPath);
                Debug.Log($"Datei erfolgreich gelesen. Größe: {fileData.Length} Bytes");

                bool headerFinish = false;

                foreach (string line in fileData)
                {

                    if (headerFinish == true)
                    {
                        string[] werte = line.Split(' ');

                        int xCord = int.Parse(werte[0]);
                        int yCord = int.Parse(werte[1]);
                        int zCord = int.Parse(werte[2]);
                        int r = int.Parse(werte[3]);
                        int g = int.Parse(werte[4]);
                        int b = int.Parse(werte[5]);

                        content[0][xCord, yCord, zCord] = (r << 24) + (g << 16) + (b << 8) + 255;

                    }

                    if (line == "end_header")
                    {
                        headerFinish = true;
                    }

                }
            }
            else
            {
                Debug.LogError($"Datei nicht gefunden: {fullPath}");
            }

        }
    }
}
