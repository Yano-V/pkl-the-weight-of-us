using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoveAnimationOffset))]
[CanEditMultipleObjects]
public class MoveAnimationOffsetEditor : UIAnimationBaseEditor<MoveAnimationOffsetEditor>
{
    private MoveAnimationOffset moveAnimationOffset;

    private SerializedProperty positionOffset;
    private SerializedProperty positionOffsetExit;
    private SerializedProperty useExitPosition;

    private void OnEnable()
    {
        moveAnimationOffset = GetReference<MoveAnimationOffset>();
        monoScript = MonoScript.FromScriptableObject(moveAnimationOffset);
        FindBaseProperties();
        FindMoveAnimationProperties();

        moveAnimationOffset.SetAnimationID();
    }

    void FindMoveAnimationProperties()
    {
        positionOffset = serializedObject.FindProperty("positionOffset");
        positionOffsetExit = serializedObject.FindProperty("positionOffsetExit");
        useExitPosition = serializedObject.FindProperty("useExitPosition");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawButtonHelpers();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawMonoScriptField();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawMoveAnimationProperties();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawBaseProperties();

        serializedObject.ApplyModifiedProperties();

        moveAnimationOffset.AutoSetReferenceSettings();
    }

    void DrawMoveAnimationProperties()
    {
        EditorGUILayout.LabelField("Move Animation", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(positionOffset);

        EditorGUILayout.PropertyField(useExitPosition);

        if (useExitPosition.boolValue)
        {
            EditorGUILayout.PropertyField(positionOffsetExit);
        }
    }
}