using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIAnimatorGroupID))]
public class UIAnimatorGroupIDEditor : Editor
{
    private UIAnimatorGroupID uIAnimatorGroupID;

    private void OnEnable()
    {
        uIAnimatorGroupID = (UIAnimatorGroupID)target;
    }

    public override void OnInspectorGUI()
    {
        DrawButtonHelpers();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawDefaultInspector();

        uIAnimatorGroupID.AutoSetReferenceSettings();
    }

    void DrawButtonHelpers()
    {
        if (GUILayout.Button("Set Animation ID by Asset"))
        {
            uIAnimatorGroupID.SetAnimatorGroupID();
        }
    }
}