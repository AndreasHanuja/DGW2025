using Game.Map.Models;
using Game.Map.WFC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            for (int i = 0; i < 8; i++)
            {
                prefabs.Add(setup.createdPrefabs[i]);
            }
        }

        prefabs.OrderBy(p => -p.collapsePriority);

        for(int i =0; i < prefabs.Count; i++)
        {
            prefabs[i].id = i;
        }
        WFCPresenter.Instance.SetPlyModelPrefabs(prefabs);
    }
}
