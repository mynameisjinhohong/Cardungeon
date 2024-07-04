using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(GameBoard_PCI))]
public class GameBoardEditor_PCI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int headCount = 0;

        GameBoard_PCI gameBoard = (GameBoard_PCI)target;
        if (GUILayout.Button("Generate"))
        {
            headCount = GamePlayManager.Instance.testValue;
            
            int rand = Random.Range(0, 100);
            // 일단 생성기는 비활성화
            gameBoard.Generate(rand, headCount);
        }
    }
}
#endif
