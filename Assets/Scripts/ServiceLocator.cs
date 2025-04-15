using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static Dictionary<Type, object> services = new();

    public static void Register<T>(T service)
    {
        if (service == null)
            throw new Exception($"Service of type {typeof(T).Name} is null and not registered.");

        var type = typeof(T);

        if (services.ContainsKey(type))
            return;

        services[type] = service;
    }

    public static T Get<T>() where T : new()
    {
        var type = typeof(T);

        if (!services.ContainsKey(type))
        {
            T newService = new();
            Register(newService);
        }

        return (T)services[type];
    }

    public static T? GetOnly<T>() where T : class
    {
        var type = typeof(T);

        if (!services.TryGetValue(type, out var service))
            return null;

        return service as T;
    }

    public static void Unregister<T>()
    {
        var type = typeof(T);

        if (services.ContainsKey(type))
        {
            services.Remove(type);
        }
    }

    public static void Clear()
    {
        services.Clear();
    }
}