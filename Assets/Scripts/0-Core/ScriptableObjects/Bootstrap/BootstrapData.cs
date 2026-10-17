using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BootstrapData", menuName = "Scriptable Objects/BootstrapData")]
public class BootstrapData : ScriptableObject
{
    public List<GameObject> MonoBehaviourServicesPrefabs;

#if UNITY_EDITOR
    public GameObject PlaceManagersHere;
#endif

#if UNITY_EDITOR
    private void OnValidate()
    {
        MonoBehaviourServicesPrefabs ??= new List<GameObject>();

        for (int i = MonoBehaviourServicesPrefabs.Count - 1; i >= 0; i--)
        {
            if (MonoBehaviourServicesPrefabs[i] == null)
            {
                MonoBehaviourServicesPrefabs.RemoveAt(i);
            }
        }

        if (PlaceManagersHere != null)
        {
            MonoBehaviourServicesPrefabs.Add(PlaceManagersHere);
            PlaceManagersHere = null;
        }
    }
#endif
}
