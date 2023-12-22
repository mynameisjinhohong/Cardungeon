using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

[CustomEditor(typeof(GameBoard_PCI))]
public class GameBoardEditor_PCI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameBoard_PCI gameBoard = (GameBoard_PCI)target;
        if (GUILayout.Button("Generate"))
        {
            int rand = Random.Range(0, 100);
            gameBoard.Generate(rand);
        }
    }
}
