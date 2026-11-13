using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RotateAnimation))]
[CanEditMultipleObjects]
public class RotateAnimationEditor : UIAnimationBaseEditor<RotateAnimationEditor>
{
    private RotateAnimation rotateAnimation;

    private SerializedProperty setStartingRotationToInitialState;
    private SerializedProperty startingRotation;
    private SerializedProperty targetRotation;
    private SerializedProperty targetRotationExit;
    private SerializedProperty useExitRotation;

    private void OnEnable()
    {
        rotateAnimation = GetReference<RotateAnimation>();
        monoScript = MonoScript.FromScriptableObject(rotateAnimation);
        FindBaseProperties();
        FindRotateAnimationProperties();

        rotateAnimation.SetAnimationID();
    }

    void FindRotateAnimationProperties()
    {
        setStartingRotationToInitialState = serializedObject.FindProperty("setStartingRotationToInitialState");
        startingRotation = serializedObject.FindProperty("startingRotation");
        targetRotation = serializedObject.FindProperty("targetRotation");
        targetRotationExit = serializedObject.FindProperty("targetRotationExit");
        useExitRotation = serializedObject.FindProperty("useExitRotation");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawButtonHelpers();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawMonoScriptField();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawRotateAnimationProperties();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawBaseProperties();

        serializedObject.ApplyModifiedProperties();

        rotateAnimation.AutoSetReferenceSettings();
    }

    void DrawRotateAnimationProperties()
    {
        EditorGUILayout.LabelField("Rotate Animation", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(setStartingRotationToInitialState);
        EditorGUILayout.PropertyField(startingRotation);
        EditorGUILayout.PropertyField(targetRotation);

        EditorGUILayout.PropertyField(useExitRotation);

        if (useExitRotation.boolValue)
        {
            EditorGUILayout.PropertyField(targetRotationExit);
        }
    }
}
