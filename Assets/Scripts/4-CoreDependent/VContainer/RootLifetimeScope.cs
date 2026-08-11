using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RootLifetimeScope : LifetimeScope
{
    [SerializeField] InputService inputService;

    protected override void Configure(IContainerBuilder builder)
    {
        Debug.Log("Running RootLifetimeScope");

        builder.RegisterComponentInNewPrefab(inputService, Lifetime.Singleton).As<IInputService>();
    }
}
