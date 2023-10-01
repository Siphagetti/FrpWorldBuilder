using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class ServiceManager
    {
        public static ServiceManager Instance { get; private set; }
        private Dictionary<Type, IBaseService> _services = new Dictionary<Type, IBaseService>();

        public ServiceManager()
        {
            if (Instance != null) return;
            Instance = this;

            // Default services
            AddService<World.IWorldService>(new World.WorldService());
            AddService<Prefab.IPrefabService>(new Prefab.PrefabService());
        }

        public void AddService<T>(T service) where T : IBaseService
        {
            if (_services.ContainsKey(service.GetType()))
            {
                Debug.LogError("Service Manager already has a services with type of " + service.GetType());
                return;
            }
            _services.Add(typeof(T), service);
            Debug.Log(typeof(T) + " added to service");
        }

        public T GetService<T>() where T : IBaseService
        {
            if (_services.TryGetValue(typeof(T), out var service)) return (T)service;
            return default;
        }

        public void UpdateService<T>(T service) where T : IBaseService
        {
            if (_services.ContainsKey(typeof(T))) _services[typeof(T)] = service;
        }

        public IEnumerable<IBaseService> GetAll() { return _services.Values; }
    }
}