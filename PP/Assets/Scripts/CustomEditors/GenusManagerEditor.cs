using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)
    [CustomEditor(typeof(GenusManager))]
    public class GenusManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GenusManager myTarget = (GenusManager)target;
            if (GUILayout.Button("Update Managers")){
                myTarget.UpdateManagers();
            }

            if (GUILayout.Button("Reset All Species to Default")){
                myTarget.UpdateManagers();
                for (int i = 0; i < myTarget.managers.Length; i++)
                {
                myTarget.managers[i].ResetVariables(); 
                }
                
            }

            DrawDefaultInspector();

            if (GUILayout.Button("Kill All")){
                myTarget.KillAll();
            }
        }
    }
#endif
