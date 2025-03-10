
using System.Collections.Generic;

namespace Game.Map.Models
{
    public class PlyModelPrefab
    {
        public const int modelSize = 16;
        public int[,,] data = new int[modelSize, modelSize, modelSize];
        /*
         * Reihenfolge der Seiten
         * 0: vorne
         * 1: hinten
         * 2: unten
         * 3: oben
         * 4: links
         * 5: rechts
         */
        public string[] seitenHashs = new string[6];

        public List<int> allowedInputs = new List<int>();
    }
}