using Game.Map.Models;
using Game.Map.Voxel;
using Game.Map.WFC;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ModelListe : MonoBehaviour
{

    [SerializeField] private List<PlyModelSetup> models = new();
    [HideInInspector] public List<PlyModelPrefab> prefabs = new();

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);

        foreach (PlyModelSetup setup in models)
		{
#if UNITY_EDITOR
			setup.LoadModel();
#endif
            setup.SetHashs();
            for (int i = 0; i < setup.createdPrefabs.Count; i++)
            {
                prefabs.Add(setup.createdPrefabs[i]);
            }
        }

        prefabs.OrderBy(p => -p.weight);

        for(int i =0; i < prefabs.Count; i++)
        {
            prefabs[i].id = i;
        }

        PlyModelPrefab emptyPrefab = new PlyModelPrefab();
        emptyPrefab.allowedGround = new List<int> { 0, 1, 2, 3 };
        emptyPrefab.allowedInputs = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        emptyPrefab.hashes = new string[] { 
            "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"};
        emptyPrefab.id = prefabs.Count;
        emptyPrefab.data = new int[0];
        prefabs.Add(emptyPrefab);


        WFCManager.Instance.WFC_Init(12, 0, prefabs, GenerateMap());
    }

    private byte[,] GenerateMap()
    {
        byte[,] data = new byte[12, 12];
       
        //unten links
        data[0, 0] = 3;
        data[0, 1] = 3;
        data[1, 0] = 3;
        data[1, 1] = 3;

        //oben links
        data[0, 11] = 3;

        //see oben links
        data[3, 8] = 3;

        //berg
        data[7, 0] = 3;
        data[7, 1] = 3;
        data[8, 0] = 3;
        data[8, 1] = 3;
        data[9, 0] = 3;
        data[9, 1] = 3;
        data[10, 0] = 3;
        data[10, 1] = 3;
        data[10, 2] = 3;
        data[11, 0] = 3;
        data[11, 1] = 3;
        data[11, 2] = 3;

        //see am leuchtturm
        data[10, 10] = 3;
        data[10, 9] = 3;

        return data;
    }
}
