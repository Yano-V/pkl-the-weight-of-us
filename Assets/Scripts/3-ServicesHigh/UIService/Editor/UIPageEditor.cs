using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIPage))]
[CanEditMultipleObjects]
public class UIPageEditor : Editor
{
    private UIPage uIPage;

    // public override bool RequiresConstantRepaint() => true;

    private void OnEnable()
    {
        uIPage = (UIPage)target;
    }

    public override void OnInspectorGUI()
    {
        DrawUtilityButtonsInspector();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawDefaultInspector();

        uIPage.AutoSetReferenceSettings();
    }

    void DrawUtilityButtonsInspector()
    {
        if (GUILayout.Button("Set Root and Content Name"))
            uIPage.SetRootContentName();
    }
}