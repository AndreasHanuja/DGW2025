using System.IO;
using UnityEngine;

namespace Game.Map
{
    [CreateAssetMenu(fileName = "PlyModel", menuName = "ScriptableObjects/PlyModel", order = 1)]
    public class PlyModel : ScriptableObject
    {
        [SerializeField] private string filepath;
        private int[,,] content = new int[16, 16, 16];

        public void LoadModel()
        {
            string fullPath = Path.Combine(Application.dataPath, filepath);

            if (File.Exists(fullPath))
            {
                string[] fileData = File.ReadAllLines(fullPath);
                Debug.Log($"Datei erfolgreich gelesen. Größe: {fileData.Length} Bytes");
            }
            else
            {
                Debug.LogError($"Datei nicht gefunden: {fullPath}");
            }

            int r = 123;
            int g = 110;
            int b = 0;

            int result = r << 16 + g << 8 + b;
        }
    }
}
