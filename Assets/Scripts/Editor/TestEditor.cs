using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    float labelWidth = 150f;

    string playerName = "Player 1";
    string playerLevel = "1";
    string playerElo = "5";
    string playerScore = "100";

    // OnInspector GUI
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Custom form for Player Preferences
        Test test = (Test)target; //1

        GUILayout.Space(20f); //2
        GUILayout.Label("Custom Editor Elements", EditorStyles.boldLabel); //3

        GUILayout.Space(10f);
        GUILayout.Label("Player Preferences");

        GUILayout.BeginHorizontal(); //4
        GUILayout.Label("Player Name", GUILayout.Width(labelWidth)); //5
        playerName = GUILayout.TextField(playerName); //6
        GUILayout.EndHorizontal(); //7

        GUILayout.BeginHorizontal();
        GUILayout.Label("Player Level", GUILayout.Width(labelWidth));
        playerLevel = GUILayout.TextField(playerLevel);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Player Elo", GUILayout.Width(labelWidth));
        playerElo = GUILayout.TextField(playerElo);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Player Score", GUILayout.Width(labelWidth));
        playerScore = GUILayout.TextField(playerScore);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save")) //8
        {
            PlayerPrefs.SetString("PlayerName", playerName); //9
            PlayerPrefs.SetString("PlayerLevel", playerLevel);
            PlayerPrefs.SetString("PlayerElo", playerElo);
            PlayerPrefs.SetString("PlayerScore", playerScore);

            Debug.Log("PlayerPrefs Saved");
        }

        if (GUILayout.Button("Reset")) //10
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs Reset");
        }

        GUILayout.EndHorizontal();
    }
}
