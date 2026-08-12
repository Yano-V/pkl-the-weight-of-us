using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraService))]
public class CameraServiceEditor : Editor
{
    private CameraService cameraService;

    private bool showDictionary;

    public override bool RequiresConstantRepaint() => true;

    private void OnEnable()
    {
        cameraService = (CameraService)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawDictionaryDisplay();
    }

    void DrawDictionaryDisplay()
    {
        showDictionary = EditorGUILayout.Foldout(showDictionary, "Registered Cameras", true);
        if (showDictionary)
        {
            foreach (var value in cameraService.RegisteredCams)
            {
                EditorGUILayout.LabelField($"ID: {value.Key} | Type: {value.Value.GetType().Name}, GameObject: {value.Value.gameObject.name}");
            }
        }
    }
}