
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class RemoveMissingScripts : EditorWindow
{
    [MenuItem("Tools/Cleanup/Remove Missing Scripts")]
    private static void ShowWindow()
    {
        GetWindow<RemoveMissingScripts>("Missing Scripts");
    }

    private void OnGUI()
    {
        GUILayout.Label(
            "Remove Missing MonoBehaviour Scripts",
            EditorStyles.boldLabel
        );

        EditorGUILayout.HelpBox(
            "Removes missing script components from GameObjects " +
            "in the selected scene or all loaded scenes.",
            MessageType.Info
        );

        if (GUILayout.Button("Clean Current Scene"))
        {
            CleanScene(SceneManager.GetActiveScene());
        }

        if (GUILayout.Button("Clean All Loaded Scenes"))
        {
            if (EditorUtility.DisplayDialog(
                "Remove Missing Scripts",
                "Clean all currently loaded scenes?",
                "Clean",
                "Cancel"))
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    CleanScene(SceneManager.GetSceneAt(i));
                }
            }
        }
    }

    private static void CleanScene(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
            return;

        int removedCount = 0;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Transform[] transforms =
                root.GetComponentsInChildren<Transform>(true);

            foreach (Transform t in transforms)
            {
                GameObject go = t.gameObject;

                int count = GameObjectUtility
                    .GetMonoBehavioursWithMissingScriptCount(go);

                if (count > 0)
                {
                    Undo.RegisterCompleteObjectUndo(
                        go, "Remove Missing Scripts");

                    GameObjectUtility
                        .RemoveMonoBehavioursWithMissingScript(go);

                    EditorUtility.SetDirty(go);
                    removedCount += count;
                }
            }
        }

        if (removedCount > 0)
        {
            EditorSceneManager.MarkSceneDirty(scene);
        }

        Debug.Log(
            $"Scene '{scene.name}': Removed {removedCount} missing script(s)."
        );
    }
}
