using Game.Map.Models;
using Game.Map.Voxel;
using Game.Map.WFC;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ModelListe : MonoBehaviour
{

    [SerializeField] private List<PlyModelSetup> models = new();
    private List<PlyModelPrefab> prefabs = new();

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);


        foreach (PlyModelSetup setup in models)
        {
            if(setup.createdPrefabs.Count != 8)
            {
                setup.LoadModel();
                setup.SetHashs();
            }
            for (int i = 0; i < 8; i++)
            {
                prefabs.Add(setup.createdPrefabs[i]);
            }
        }

        prefabs.OrderBy(p => -p.weight);

        for(int i =0; i < prefabs.Count; i++)
        {
            prefabs[i].id = i;
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        IEnumerable<WFCOutputChange> output = WFCManager.Instance.WFC_Init(12, 0, prefabs);
        UnityEngine.Debug.Log("WFC took " + stopwatch.ElapsedMilliseconds);

        stopwatch.Restart();
        Parallel.ForEach(output, o => VoxelPresenter.Instance.SetStructure(new Vector3Int(o.position.x * 16, 0, o.position.y * 16), prefabs[o.value].data));
        UnityEngine.Debug.Log("Meshing took " + stopwatch.ElapsedMilliseconds);

    }
}
