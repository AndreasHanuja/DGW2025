using UnityEngine;
using UnityEditor;
using Game.Map;

[CustomEditor(typeof(PlyModel))]
public class PlyModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlyModel plyModel = (PlyModel)target;

        if (GUILayout.Button("Load Model"))
        {
            plyModel.LoadModel();
        }
    }
}
