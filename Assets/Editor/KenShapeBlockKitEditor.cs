using System.IO;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(KenShapeBlockKit))]
public class KenShapeBlockKitEditor : Editor {
    private KenShapeBlockKit _target {
        get {
            return (KenShapeBlockKit)target;
        }
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Split Into Blocks")) {
            _target.GenerateModel();
        }
    }
}