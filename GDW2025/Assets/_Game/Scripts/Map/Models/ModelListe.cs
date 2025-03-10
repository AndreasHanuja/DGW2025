using Game.Map.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    }

}
