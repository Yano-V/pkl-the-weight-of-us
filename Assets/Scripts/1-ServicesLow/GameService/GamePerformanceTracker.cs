using System;
using UnityEngine;

[Serializable]
public class GamePerformanceTracker
{
    private const int FrameTimeBufferSize = 60;

    private readonly IGameServiceManageable iGSM;
    private readonly float[] recentFrameTimes = new float[FrameTimeBufferSize];

    [SerializeField] float rawFrameTime;
    [SerializeField] int rawFrameRate;
    [SerializeField] float averageFrameTime;
    [SerializeField] int averageFrameRate;

    private float recentFrameTimeTotal;
    private int nextFrameTimeIndex;
    private int frameTimeSampleCount;

    public float RawFrameTime => rawFrameTime;
    public int RawFrameRate => rawFrameRate;
    public float AverageFrameTime => averageFrameTime;
    public int AverageFrameRate => averageFrameRate;

    public GamePerformanceTracker(IGameServiceManageable iGSM)
    {
        this.iGSM = iGSM;
    }

    public void Update(float deltaTime)
    {
        if (deltaTime <= 0f)
            return;

        UpdateFrameTime(deltaTime);
        UpdateFrameRate(deltaTime);
    }

    private void UpdateFrameTime(float deltaTime)
    {
        rawFrameTime = deltaTime * 1000f;

        if (frameTimeSampleCount == recentFrameTimes.Length)
        {
            recentFrameTimeTotal -= recentFrameTimes[nextFrameTimeIndex];
        }
        else
        {
            frameTimeSampleCount++;
        }

        recentFrameTimes[nextFrameTimeIndex] = deltaTime;
        recentFrameTimeTotal += deltaTime;
        nextFrameTimeIndex = (nextFrameTimeIndex + 1) % recentFrameTimes.Length;

        averageFrameTime = recentFrameTimeTotal / frameTimeSampleCount * 1000f;
    }

    private void UpdateFrameRate(float deltaTime)
    {
        rawFrameRate = Mathf.RoundToInt(1f / deltaTime);
        averageFrameRate = Mathf.RoundToInt(frameTimeSampleCount / recentFrameTimeTotal);
    }
}