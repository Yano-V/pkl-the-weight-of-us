using UnityEngine;

public interface IUIService : IBaseService
{
    UIServiceSettingsSO UIServiceSettingsSO { get; }

    UIPageHandler UIPageHandler { get; }
    Canvas LoadingUICanvas { get; }
}
