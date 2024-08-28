using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEditor.UIElements;
using System.Threading;

[CustomEditor(typeof(Generator2D))]
public class DGUI : Editor
{
    public override void OnInspectorGUI()
    {
        Generator2D dgener = (Generator2D)target;
        
        DrawDefaultInspector();
        if (GUILayout.Button("dg"))
        {
            dgener.multiLayerGenerate();
        }

        if (GUILayout.Button("clear"))
        {
            dgener.ClearScene();
        }

    }
}
