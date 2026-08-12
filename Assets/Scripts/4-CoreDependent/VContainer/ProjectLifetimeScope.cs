using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectLifetimeScope : LifetimeScope
{
    [SerializeField] CameraService cameraService;
    [SerializeField] InputService inputService;

    protected override void Configure(IContainerBuilder builder)
    {
        Debug.Log("Running RootLifetimeScope");

        builder.Register<MainGameInput>(Lifetime.Transient);

        builder.RegisterComponentInNewPrefab(cameraService, Lifetime.Singleton)
            .DontDestroyOnLoad()
            .As<ICameraService>();

        builder.RegisterComponentInNewPrefab(inputService, Lifetime.Singleton)
            .DontDestroyOnLoad()
            .As<IInputService>();
    }
}
