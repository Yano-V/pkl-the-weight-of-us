using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Limbo UI Service Settings", menuName = "Limbo/UI/UI Service Settings")]
public class UIServiceSettingsSO : ScriptableObject
{
    [SerializeField] bool debugMode;
    [SerializeField] bool dontSendLoadingPageDoneEvent;

    [Header("Loading Page Prefabs")]
    [SerializeField] List<GameObject> loadingPagePrefabs;

    public bool DebugMode => debugMode;
    public bool DontSendLoadingPageDoneEvent => debugMode && dontSendLoadingPageDoneEvent;

    public IReadOnlyList<GameObject> LoadingPagePrefabs => loadingPagePrefabs;
}