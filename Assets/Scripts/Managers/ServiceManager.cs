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

            // Default services
            AddService<Language.ILanguageService>       (new Language.LanguageService());
            AddService<Prefab.IPrefabService>           (new Prefab.PrefabService());
            AddService<World.IWorldService>             (new World.WorldService());
        }

        public static void AddService<T>(T service) where T : IBaseService
        {
            if (_instance._services.ContainsKey(service.GetType()))
            {
                Debug.LogError("Service Manager already has a services with type of " + service.GetType());
                return;
            }
            _instance._services.Add(typeof(T), service);
        }

        public static T GetService<T>() where T : IBaseService
        {
            if (_instance._services.TryGetValue(typeof(T), out var service)) return (T)service;
            return default;
        }

        public static void UpdateService<T>(T service) where T : IBaseService
        {
            if (_instance._services.ContainsKey(typeof(T))) _instance._services[typeof(T)] = service;
        }

        public IEnumerable<IBaseService> GetAll() { return _services.Values; }
    }
}