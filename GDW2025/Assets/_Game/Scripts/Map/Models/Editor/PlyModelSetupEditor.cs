using UnityEditor;
using UnityEngine;

namespace Game.Map.Models
{
    [CustomEditor(typeof(PlyModelSetup))]
    public class PlyModelSetupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PlyModelSetup plyModel = (PlyModelSetup)target;

            if (GUILayout.Button("Load Model"))
            {
                plyModel.LoadModel();
            }
        }
    }
}