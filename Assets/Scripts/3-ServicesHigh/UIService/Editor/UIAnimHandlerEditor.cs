using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIAnimHandler))]
[CanEditMultipleObjects]
public class UIAnimHandlerEditor : Editor
{
    private bool showDebug;

    private UIAnimHandler uIAnimHandler;

    public override bool RequiresConstantRepaint() => showDebug;

    private void OnEnable()
    {
        uIAnimHandler = (UIAnimHandler)target;
    }

    public override void OnInspectorGUI()
    {
        uIAnimHandler.AutoSetReferenceSettings();

        DrawDefaultInspector();
        // EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}