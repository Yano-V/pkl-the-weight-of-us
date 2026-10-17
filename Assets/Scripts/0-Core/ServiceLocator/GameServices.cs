using System;
using System.Collections.Generic;
using PKLStudios;
using UnityEngine;

public static class GameServices
{
    static readonly Dictionary<Type, IBaseService> services = new();
    public static IReadOnlyDictionary<Type, IBaseService> Services => services;

    /// <summary>
    /// Registers a service.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    /// <param name="service">The service to register.</param>
    public static void Register<T>(T service) where T : IBaseService
    {
        Type type = typeof(T);

        if (services.ContainsKey(type))
        {
            LogUtilities.LogError($"Service {type.Name} is already registered", "GameServices");
            return;
        }

        services.Add(type, service);
        LogUtilities.Log($"Service {type.Name} registered", "GameServices");
    }

    //public static void RegisterServiceLazy<T>() where T : class
    //{

    //}

    /// <summary>
    /// Unregisters a service.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    public static void Unregister<T>() where T : class
    {
        Type type = typeof(T);

        if (!services.ContainsKey(type))
        {
            LogUtilities.LogError($"Service {type.Name} is not registered", "GameServices");
            return;
        }

        services.Remove(type);

        LogUtilities.Log($"Service {type.Name} unregistered", "GameServices");
    }

    /// <summary>
    /// Gets a service.
    /// </summary>
    /// <remarks>
    /// Unsafe method. Throws an exception if the service is not found.
    /// This is used to follow the fail-fast principle.
    /// </remarks>
    /// <typeparam name="T">The type of the service.</typeparam>
    /// <returns>The service.</returns>
    public static T Get<T>() where T : IBaseService
    {
        Type type = typeof(T);
        
        if (!services.TryGetValue(type, out IBaseService service))
        {
            throw new MissingServiceException(type);
        }

        return (T)service;
    }

    public static bool IsServiceRegistered<T>() where T : IBaseService
    {
        return services.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Resets the services by clearing the dictionary.
    /// </summary>
    public static void ResetServices()
    {
        services.Clear();
    }
}
