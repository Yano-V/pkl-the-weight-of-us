using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScaleAnimation))]
[CanEditMultipleObjects]
public class ScaleAnimationEditor : UIAnimationBaseEditor<ScaleAnimationEditor>
{
    private ScaleAnimation scaleAnimation;

    private SerializedProperty setStartingScaleToInitialState;
    private SerializedProperty startingScale;
    private SerializedProperty targetScale;
    private SerializedProperty targetScaleExit;
    private SerializedProperty useExitScale;

    private void OnEnable()
    {
        scaleAnimation = GetReference<ScaleAnimation>();
        monoScript = MonoScript.FromScriptableObject(scaleAnimation);
        FindBaseProperties();
        FindScaleAnimationProperties();

        scaleAnimation.SetAnimationID();
    }

    void FindScaleAnimationProperties()
    {
        setStartingScaleToInitialState = serializedObject.FindProperty("setStartingScaleToInitialState");
        startingScale = serializedObject.FindProperty("startingScale");
        targetScale = serializedObject.FindProperty("targetScale");
        targetScaleExit = serializedObject.FindProperty("targetScaleExit");
        useExitScale = serializedObject.FindProperty("useExitScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawButtonHelpers();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawMonoScriptField();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawScaleAnimationProperties();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawBaseProperties();

        serializedObject.ApplyModifiedProperties();

        scaleAnimation.AutoSetReferenceSettings();
    }

    void DrawScaleAnimationProperties()
    {
        EditorGUILayout.LabelField("Scale Animation", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(setStartingScaleToInitialState);
        EditorGUILayout.PropertyField(startingScale);
        EditorGUILayout.PropertyField(targetScale);

        EditorGUILayout.PropertyField(useExitScale);

        if (useExitScale.boolValue)
        {
            EditorGUILayout.PropertyField(targetScaleExit);
        }
    }
}
