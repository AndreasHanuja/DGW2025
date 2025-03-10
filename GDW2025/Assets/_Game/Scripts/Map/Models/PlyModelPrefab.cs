
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Map.Models
{
    [Serializable]
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
        public int collapsePriority;

        public PlyModel Instantiate(Vector3Int offset)
        {
            return new PlyModel(offset, this);
        }

        public bool CheckConnectivity(PlyModelPrefab other, Vector3Int direction)
        {
            // von vorne
            if(direction == new Vector3Int(0, 0, 1))
            {
                return seitenHashs[0] == other.seitenHashs[1];
            }
            // von hinten
            if (direction == new Vector3Int(0, 0, -1))
            {
                return seitenHashs[1] == other.seitenHashs[0];
            }
            // von unten
            if (direction == new Vector3Int(0, 1, 0))
            {
                return seitenHashs[2] == other.seitenHashs[3];
            }
            // von oben
            if (direction == new Vector3Int(0, -1, 0))
            {
                return seitenHashs[3] == other.seitenHashs[2];
            }
            // von links
            if (direction == new Vector3Int(1, 0, 0))
            {
                return seitenHashs[4] == other.seitenHashs[5];
            }
            // von rechts
            if (direction == new Vector3Int(-1, 0, 0))
            {
                return seitenHashs[5] == other.seitenHashs[4];
            }

            return false;
        }
    }


}