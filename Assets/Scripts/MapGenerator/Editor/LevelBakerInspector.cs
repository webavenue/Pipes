using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(LevelBaker))]
public class LevelBakerInspector : Editor
{
    int first;
    int second;
    string assetName = "";

    public LevelBaker levelBaker
    {
        get
        {
            return target as LevelBaker;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        first = EditorGUILayout.IntField(first, GUILayout.Width(50));
        second = EditorGUILayout.IntField(second, GUILayout.Width(50));
        assetName = EditorGUILayout.TextField(assetName);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Bake!"))
        {
            levelBaker.Bake(assetName, new Tuple<int, int>(first, second));
        }
    }
}
