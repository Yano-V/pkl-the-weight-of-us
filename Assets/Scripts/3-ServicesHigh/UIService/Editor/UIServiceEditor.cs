using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIService))]
public class UIServiceEditor : Editor
{
    private UIService uIService;

    private bool showUIPageHandlerEntries;

    public override bool RequiresConstantRepaint() => true;

    private void OnEnable()
    {
        uIService = (UIService)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        DrawUIPageHandlerEntries();
    }

    void DrawUIPageHandlerEntries()
    {
        showUIPageHandlerEntries = EditorGUILayout.Foldout(showUIPageHandlerEntries, "UI Page Handler Dictionary Entries", true);
        if (showUIPageHandlerEntries)
        {
            EditorGUILayout.LabelField("Registered Pages", EditorStyles.boldLabel);
            if (uIService.UIPageHandler != null &&
                uIService.UIPageHandler.RegisteredPages != null &&
                uIService.UIPageHandler.RegisteredPages.Count > 0)
            {
                int num = 0;
                foreach (var kvp in uIService.UIPageHandler.RegisteredPages)
                {
                    EditorGUILayout.LabelField($"{num + 1}: {kvp.Key} | {kvp.Value.PageID}");
                    num++;
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Loading Pages", EditorStyles.boldLabel);
            if (uIService.UIPageHandler != null &&
                uIService.UIPageHandler.LoadingPages != null &&
                uIService.UIPageHandler.LoadingPages.Count > 0)
            {
                int num = 0;
                foreach (var kvp in uIService.UIPageHandler.LoadingPages)
                {
                    EditorGUILayout.LabelField($"{num + 1}: {kvp.Key} | {kvp.Value.PageID}");
                    num++;
                }
            }

        }
    }
}