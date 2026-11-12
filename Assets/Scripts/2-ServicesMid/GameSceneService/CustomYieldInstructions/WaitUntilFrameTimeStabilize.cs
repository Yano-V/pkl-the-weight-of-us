using UnityEngine;

public class WaitUntilFrameTimeStabilize : CustomYieldInstruction
{
    private readonly IGameService iGS; // For later use
    private readonly ISceneService iSS;

    private const float StabilityTolerancePercent = 0.1f;
    private const int RequiredStableFrameCount = 10;

    private int stableFrameCount;

    private float timeoutTimer;

    public float CachedAvgFrameTime { get; private set; }
    public float CurrentFrameTime { get; private set; }

    public override bool keepWaiting => stableFrameCount < RequiredStableFrameCount &&
                                        timeoutTimer < iSS.SceneServiceSettingsSO.FrameTimeStableCheckTimeout;

    public WaitUntilFrameTimeStabilize(IGameService iGS, ISceneService iSS)
    {
        this.iGS = iGS;
        this.iSS = iSS;
    }

    public void PrepareYieldInstruction(float cachedFrameTime)
    {
        CachedAvgFrameTime = cachedFrameTime;
        CurrentFrameTime = cachedFrameTime;
        stableFrameCount = 0;
    }

    public void ResetTimeoutTimer() => timeoutTimer = 0f;

    public void UpdateFrameTime(float frameTime)
    {
        CurrentFrameTime = frameTime;

        if (frameTime <= 0f || float.IsNaN(frameTime) || float.IsInfinity(frameTime))
        {
            stableFrameCount = 0;
            return;
        }

        if (CachedAvgFrameTime <= 0f || float.IsNaN(CachedAvgFrameTime) || float.IsInfinity(CachedAvgFrameTime))
            CachedAvgFrameTime = frameTime;

        float difference = Mathf.Abs(CurrentFrameTime - CachedAvgFrameTime);
        float tolerance = CachedAvgFrameTime * StabilityTolerancePercent;
        stableFrameCount = difference <= tolerance ? stableFrameCount + 1 : 0;
    }

    public void UpdateTimeoutTimer(float deltaTime)
    {
        if (iGS.LoadState == LoadState.PostLoading)
            timeoutTimer += deltaTime;
    }
}