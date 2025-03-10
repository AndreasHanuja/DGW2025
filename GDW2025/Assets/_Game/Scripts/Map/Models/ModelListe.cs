using Game.Map.Models;
using Game.Map.WFC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelListe : MonoBehaviour
{

    [SerializeField] private List<PlyModelSetup> models = new();
    private List<PlyModelPrefab> prefabs = new();

    private void Start()
    {
        foreach (PlyModelSetup setup in models)
        {
            for (int i = 0; i < 8; i++)
            {
                prefabs.Add(setup.content[i]);
            }
        }

        prefabs.OrderBy(p => -p.collapsePriority);

        PlyModelPrefab emptyPrefab = new PlyModelPrefab();
        emptyPrefab.seitenHashs[0] = "26103cb22a73b4df6e6fddb6a34f2b62dae95d0e7a9a5e975ace20f5fcd683dc";
        emptyPrefab.seitenHashs[1] = "26103cb22a73b4df6e6fddb6a34f2b62dae95d0e7a9a5e975ace20f5fcd683dc";
        emptyPrefab.seitenHashs[2] = "26103cb22a73b4df6e6fddb6a34f2b62dae95d0e7a9a5e975ace20f5fcd683dc";
        emptyPrefab.seitenHashs[3] = "26103cb22a73b4df6e6fddb6a34f2b62dae95d0e7a9a5e975ace20f5fcd683dc";
        emptyPrefab.seitenHashs[4] = "26103cb22a73b4df6e6fddb6a34f2b62dae95d0e7a9a5e975ace20f5fcd683dc";
        emptyPrefab.seitenHashs[5] = "26103cb22a73b4df6e6fddb6a34f2b62dae95d0e7a9a5e975ace20f5fcd683dc";
        emptyPrefab.allowedInputs.Add(0);

        prefabs.Add(emptyPrefab);
        WFCPresenter.Instance.SetPlyModelPrefabs(prefabs);
    }
}
