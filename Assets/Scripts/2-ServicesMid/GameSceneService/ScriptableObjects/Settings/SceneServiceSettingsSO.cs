using UnityEngine;

[CreateAssetMenu(fileName = "Limbo Scene Service Settings", menuName = "Limbo/Service Settings/Scene Service Settings")]
public class SceneServiceSettingsSO : ScriptableObject
{
    [SerializeField] float frameTimeStableCheckTimeout = 2f;

    public float FrameTimeStableCheckTimeout => frameTimeStableCheckTimeout;
}
