using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIAnimator))]
[CanEditMultipleObjects]
public class UIAnimatorEditor : Editor
{
    private UIAnimator uIAnimator;

    private void OnEnable()
    {
        uIAnimator = (UIAnimator)target;
    }

    public override void OnInspectorGUI()
    {
        DrawButtonHelpers();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawDefaultInspector();

        uIAnimator.AutoSetReferenceSettings();
    }

    void DrawButtonHelpers()
    {
        if (GUILayout.Button("Refresh Animations"))
        {
            uIAnimator.ReinstantiateAnimationGroups();
        }
    }
}