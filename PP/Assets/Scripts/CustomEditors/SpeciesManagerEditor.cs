using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)
    [CustomEditor(typeof(SpeciesManager))]
    public class SpeciesManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SpeciesManager myTarget = (SpeciesManager)target;
            if (GUILayout.Button("Reset to Default")){
                myTarget.ResetVariables();
            }

            DrawDefaultInspector();

            if (GUILayout.Button("Kill All")){
                myTarget.KillAll();
            }
        }
    }
#endif
