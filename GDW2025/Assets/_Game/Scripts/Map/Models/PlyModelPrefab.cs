
using System.Collections.Generic;

namespace Game.Map.Models
{
    public class PlyModelPrefab
    {
        public const int modelSize = 16;
        public int[,,] data = new int[modelSize, modelSize, modelSize];

        public List<int> allowedInputs = new List<int>();
    }
}