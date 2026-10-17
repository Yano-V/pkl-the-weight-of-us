using UnityEngine;

public class MonoServiceBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void LoadAllServices()
    {
        BootstrapData bootstrapData = Resources.Load<BootstrapData>("BootstrapData");

        if (bootstrapData == null)
        {
            Debug.LogError("BootstrapData was not found in a Resources folder.");
            return;
        }

        foreach (GameObject service in bootstrapData.MonoBehaviourServicesPrefabs)
        {
            if (service == null)
            {
                Debug.LogError("BootstrapData contains an unassigned service prefab.");
                continue;
            }

            GameObject instance = Object.Instantiate(service);
            MonoService monoService = instance.GetComponent<MonoService>();

            if (monoService == null)
            {
                Debug.LogError($"Service prefab {service.name} does not contain a MonoService component.");
                Object.Destroy(instance);
                continue;
            }

            instance.name = $"<{monoService.GetType().Name}>";
            Object.DontDestroyOnLoad(instance);
        }
    }
}
