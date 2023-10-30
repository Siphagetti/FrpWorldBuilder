using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class ServiceManager
    {
        private static ServiceManager _instance;
        private Dictionary<Type, IBaseService> _services = new Dictionary<Type, IBaseService>();

        public ServiceManager()
        {
            if (_instance != null) return;
            _instance = this;

            // Initialize default services
            AddService<Language.ILanguageService>(new Language.LanguageService());
            AddService<Prefab.IPrefabService>(new Prefab.PrefabService());
            AddService<Hierarchy.ISceneService>(new Hierarchy.SceneService());
        }

        public static void AddService<T>(T service) where T : IBaseService
        {
            // Add a new service to the service manager
            if (_instance._services.ContainsKey(service.GetType()))
            {
                Debug.LogError("Service Manager already has a service with the type of " + service.GetType());
                return;
            }
            _instance._services.Add(typeof(T), service);
        }

        public static T GetService<T>() where T : IBaseService
        {
            // Get a service of the specified type from the service manager
            if (_instance._services.TryGetValue(typeof(T), out var service))
                return (T)service;
            return default;
        }

        public static void UpdateService<T>(T service) where T : IBaseService
        {
            // Update a service in the service manager
            if (_instance._services.ContainsKey(typeof(T)))
                _instance._services[typeof(T)] = service;
        }

        public IEnumerable<IBaseService> GetAll()
        {
            // Get all registered services
            return _services.Values;
        }
    }
}
