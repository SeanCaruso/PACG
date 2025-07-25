using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    // The dictionary that holds all our services (managers, controllers, etc.)
    private static readonly Dictionary<Type, object> services = new();

    // Register a service.
    public static void Register<T>(T service)
    {
        Type type = typeof(T);
        if (services.ContainsKey(type))
        {
            Debug.LogWarning($"Service of type {type.Name} is already registered.");
            return;
        }

        services[type] = service;
        Debug.Log($"Service registered: {type.Name}");
    }

    // Get a service.
    public static T Get<T>()
    {
        Type type = typeof(T);
        if (!services.ContainsKey(type))
        {
            Debug.LogError($"Service of type {type.Name} is not registered.");
            return default;
        }
        return (T)services[type];
    }
}
