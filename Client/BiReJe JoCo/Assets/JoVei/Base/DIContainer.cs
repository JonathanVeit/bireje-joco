using System;
using System.Collections.Generic;
using UnityEngine;
using JoVei.Base.Helper;

namespace JoVei.Base
{
    /// <summary>
    /// Dependency Injection container 
    /// </summary>
    public static class DIContainer 
    {
        public static event Action<Type> onImplementationRegistered;

        private static Dictionary<Type, object> registeredImplementations;

        public static void Setup()
        {
            registeredImplementations = new Dictionary<Type, object>();
        }

        public static void RegisterImplementation<T>(object implementation) where T : class
        {
            Type typeOfT = typeof(T);

            if (registeredImplementations.ContainsKey(typeOfT))
            {
                DebugHelper.PrintFormatted(LogType.Warning,
                    "DIContainer is unable to add implementation for Type: {0}, implementation is " +
                    "already registered. Use OverwriteImplementation() instead", typeOfT.ToString());
                return;
            }

            registeredImplementations.Add(typeOfT, implementation);
            onImplementationRegistered?.Invoke(typeOfT);
        }

        public static T GetImplementationFor<T>() where T : class
        {
            Type typeOfT = typeof(T);

            if (registeredImplementations == null)
            {
                return null;
            }

            if (registeredImplementations.ContainsKey(typeOfT))
            {
                return (T)registeredImplementations[typeOfT];
            }

            DebugHelper.PrintFormatted(LogType.Warning, "DIContainer has no implementation registered for {0}!", typeOfT.ToString());
            return null;
        }

        public static void UnregisterImplementation<T>() where T : class
        {
            Type typeOfT = typeof(T);
            if (!registeredImplementations.ContainsKey(typeOfT))
            {
                DebugHelper.PrintFormatted(LogType.Error, "DIContainer has no implementation registered for {0}!", typeOfT.ToString());
                return;
            }

            registeredImplementations.Remove(typeOfT);
        }

        public static void OverrideImplementation<T>(object newImplementation) where T : class
        {
            Type typeOfT = typeof(T);
            if (!registeredImplementations.ContainsKey(typeOfT))
            {
                DebugHelper.PrintFormatted(LogType.Error, "DIContainer has no implementation registered for type {0}!", typeOfT.ToString());
                return;
            }

            UnregisterImplementation<T>();
            RegisterImplementation<T>(newImplementation);
        }

        public static bool HasImplementation<T>() where T : class
        {
            return registeredImplementations.ContainsKey(typeof(T));
        }
    }
}
