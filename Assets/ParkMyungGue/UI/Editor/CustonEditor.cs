using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(MainGame_s))]
public class CustonEditor : Editor
{

        public MainGame_s mainGame;
        public void OnEnable()
        {
            mainGame = (MainGame_s)target;
        }
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Change Option"))
        {
            mainGame.TimeSetting();
        }
        base.OnInspectorGUI();
    }

}
#endif