using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FadeAnimation))]
[CanEditMultipleObjects]
public class FadeAnimationEditor : UIAnimationBaseEditor<FadeAnimationEditor>
{
    private FadeAnimation fadeAnimation;

    private SerializedProperty targetAlpha;
    private SerializedProperty targetAlphaExit;
    private SerializedProperty useExitAlpha;

    private void OnEnable()
    {
        fadeAnimation = GetReference<FadeAnimation>();
        monoScript = MonoScript.FromScriptableObject(fadeAnimation);
        FindBaseProperties();
        FindFadeAnimationProperties();

        fadeAnimation.SetAnimationID();
    }

    void FindFadeAnimationProperties()
    {
        targetAlpha = serializedObject.FindProperty("targetAlpha");
        targetAlphaExit = serializedObject.FindProperty("targetAlphaExit");
        useExitAlpha = serializedObject.FindProperty("useExitAlpha");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawButtonHelpers();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawMonoScriptField();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawFadeAnimationProperties();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawBaseProperties();

        serializedObject.ApplyModifiedProperties();

        fadeAnimation.AutoSetReferenceSettings();
    }

    void DrawFadeAnimationProperties()
    {
        EditorGUILayout.LabelField("Fade Animation", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(targetAlpha);

        EditorGUILayout.PropertyField(useExitAlpha);

        if (useExitAlpha.boolValue)
        {
            EditorGUILayout.PropertyField(targetAlphaExit);
        }
    }
}
