using Game.Map.Voxel;
using Game.Map.WFC;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Map
{
    public class MapInteractionManager : MonoBehaviour
    {
        [SerializeField] private Raycast raycast;
        [SerializeField] private ModelListe modelListe;

        private void Start()
        {
            raycast.click.AddListener(v => MapClickHandler(v));
        }

        private void MapClickHandler(Vector2 clickPosition)
        {
            List<WFCInputChange> inputs = new List<WFCInputChange>
            {
                new WFCInputChange() { position = Vector2Int.RoundToInt(clickPosition), Type = ChangeType.Map, value = 1 }
            };
            IEnumerable<WFCOutputChange> outputChange = WFCManager.Instance.WFC_Iterate(inputs);
            Parallel.ForEach(outputChange, o => VoxelPresenter.Instance.SetStructure(new Vector3Int(o.position.x * 16, 0, o.position.y * 16), modelListe.prefabs[o.value].data));
        }
    }
}