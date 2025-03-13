using Game.Map.Models;
using Game.Map.Voxel;
using Game.Map.WFC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Map
{
    public class MapInteractionManager : SingeltonMonoBehaviour<MapInteractionManager>
    {
        [SerializeField] private Raycast raycast;
        [SerializeField] private ModelListe modelListe;

        public struct WFCResolvedChange
        {
            public Vector2Int position;
            public PlyModelSetup oldValue;
            public PlyModelSetup newValue;

        }
        public event Action<List<WFCResolvedChange>> OnResolvedOutputChanged; 

        private void Start()
        {
            raycast.click.AddListener(v => MapClickHandler(v));
        }

        private void MapClickHandler(Vector2 clickPosition)
        {
            if(!GameManager.Instance.IsInState(GameManager.State.SelectingBuildingPlacement)) 
            {
                return;
            }
            CardStackManager.Instance.TryPeek(out byte currentCard);
            List<WFCInputChange> inputs = new List<WFCInputChange>
            {
                new WFCInputChange() { position = Vector2Int.RoundToInt(clickPosition), Type = ChangeType.Input, value = currentCard }
            };
            IEnumerable<WFCOutputChange> outputChange = WFCManager.Instance.WFC_Iterate(inputs);            
            Parallel.ForEach(outputChange, o => VoxelPresenter.Instance.SetStructure(new Vector3Int(o.position.x * 16, 0, o.position.y * 16), modelListe.prefabs[o.newValue].data));

            if (outputChange.Count() > 0)
            {
                List<PlyModelPrefab> prefabs = WFCManager.Instance.GetPrefabs();
                GameManager.Instance.PlacedBuilding(
                    outputChange.Select(o => {
                        return new WFCResolvedChange { 
                            position = o.position,  
                            newValue = o.newValue == -1 ? null : prefabs[o.newValue].setup, 
                            oldValue = o.oldValue == -1 ? null : prefabs[o.oldValue].setup
                        };
                    }).ToList());
            }
        }
    }
}