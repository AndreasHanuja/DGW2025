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

            //render new models
            Parallel.ForEach(outputChange, o => VoxelPresenter.Instance.SetStructure(new Vector3Int(o.position.x * 16, 0, o.position.y * 16), modelListe.prefabs[o.newValue].data));

            //transform bioms
            if (outputChange.Count() > 0 && currentCard == 6 || currentCard == 7) 
            {
                IEnumerable<Vector2Int> updatedPositions = WFCManager.GetNeighbours(Vector2Int.RoundToInt(clickPosition), true);
                byte targetGround = (byte)(currentCard == 6 ? 2 : 1);

                IEnumerable<WFCOutputChange> tmp = WFCManager.Instance.WFC_Iterate(
                    updatedPositions.Select(p =>
                      new WFCInputChange() { position = p, Type = ChangeType.Map, value = targetGround }
                  ).ToList());
                Parallel.ForEach(tmp, o => VoxelPresenter.Instance.SetStructure(new Vector3Int(o.position.x * 16, 0, o.position.y * 16), modelListe.prefabs[o.newValue].data));

                outputChange = outputChange.Concat(tmp);

                Parallel.ForEach(updatedPositions, p => VoxelPresenter.Instance.GenerateGroundStructure(targetGround, p));
            }

            //notify game manager
            if (outputChange.Count() > 0)
            {
                List<PlyModelPrefab> prefabs = WFCManager.Instance.GetPrefabs();

                List<WFCOutputChange> currentoutputChange = outputChange.Where(o => o.oldValue != o.newValue).ToList();
				GameManager.Instance.PlacedBuilding(
					currentoutputChange
					.Select(o => {
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