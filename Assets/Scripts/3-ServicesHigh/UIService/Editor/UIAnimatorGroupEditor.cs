using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIAnimatorGroup))]
[CanEditMultipleObjects]
public class UIAnimatorGroupEditor : Editor
{
    private UIAnimatorGroup uIAnimatorGroup;

    private void OnEnable()
    {
        uIAnimatorGroup = (UIAnimatorGroup)target;
    }

    public override void OnInspectorGUI()
    {
        DrawUtilityButtonsInspector();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawDefaultInspector();

        uIAnimatorGroup.AutoSetReferenceSettings();
    }

    void DrawUtilityButtonsInspector()
    {
        EditorGUILayout.LabelField("Utility Buttons", EditorStyles.boldLabel);

        if (GUILayout.Button("Set GameObject Name by Group ID"))
            uIAnimatorGroup.SetGameObjectName();
    }
}