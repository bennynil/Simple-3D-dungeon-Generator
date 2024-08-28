using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(roomGenerate))]
public class RGUI : Editor
{
    public override void OnInspectorGUI()
    {
        roomGenerate rg = (roomGenerate)target;

        DrawDefaultInspector();

        if (GUILayout.Button("gen"))
        {
            rg.rseed(rg.randomSeed);
            rg.ClearScene();
            rg.Gen();
        }

    }
}
