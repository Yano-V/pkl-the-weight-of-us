using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIPageSO))]
public class UIPageSOEditor : Editor
{
    private UIPageSO uIPageSO;

    private void OnEnable()
    {
        uIPageSO = (UIPageSO)target;
    }

    public override void OnInspectorGUI()
    {
        DrawUtilityButtonsInspector();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawDefaultInspector();

        uIPageSO.AutoSetReferenceSettings();
    }

    void DrawUtilityButtonsInspector()
    {
        if (GUILayout.Button("Set Asset name with Page ID"))
            uIPageSO.SetAssetNameToID();
    }
}