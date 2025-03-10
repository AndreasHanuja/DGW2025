using Game.Map.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Map.WFC
{
    public class WFCPresenter : MonoBehaviour
    {
        public event Action<Vector3Int, PlyModel> OnModelUpdated;

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

            //TODO neu errechnung output
        }
        public byte GetInput(Vector3Int position)
        {
            return model.GetInput(position);
        }
        public void SetPlyModelPrefabs(List<PlyModelPrefab> prefabs)
        {

        }
    }
}