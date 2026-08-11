using UnityEngine;

public static class LogUtilities
{
    public static void Log(string message, string className)
        => Debug.Log($"[{className}]: {message}");

    public static void Log(string message, string className, GameObject gameObject)
        => Debug.Log($"[{className}]: {message} ({gameObject.name})");

    public static void Log(string message, string className, GameObject gameObject, Vector3 position)
        => Debug.Log($"[{className}]: {message} ({gameObject.name} in {position})");

    public static void LogWarning(string message, string className)
        => Debug.LogWarning($"[{className}]: {message}");

    public static void LogWarning(string message, string className, GameObject gameObject)
        => Debug.LogWarning($"[{className}]: {message} ({gameObject.name})");

    public static void LogWarning(string message, string className, GameObject gameObject, Vector3 position)
        => Debug.LogWarning($"[{className}]: {message} ({gameObject.name} in {position})");

    public static void LogError(string message, string className)
        => Debug.LogError($"[{className}]: {message}");

    public static void LogError(string message, string className, GameObject gameObject)
        => Debug.LogError($"[{className}]: {message} ({gameObject.name})");

    public static void LogError(string message, string className, GameObject gameObject, Vector3 position)
        => Debug.LogError($"[{className}]: {message} ({gameObject.name} in {position})");

    public static void Assert(bool condition, string message, string className)
        => Debug.Assert(condition, $"[{className}]: {message}");

    public static void Assert(bool condition, string message, string className, GameObject gameObject)
        => Debug.Assert(condition, $"[{className}]: {message} ({gameObject.name})");

    public static void Assert(bool condition, string message, string className, GameObject gameObject, Vector3 position)
        => Debug.Assert(condition, $"[{className}]: {message} ({gameObject.name} in {position})");
}
