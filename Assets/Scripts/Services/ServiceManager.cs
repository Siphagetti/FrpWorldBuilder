using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class ServiceManager
    {
        private Dictionary<Type, IBaseService> _services = new Dictionary<Type, IBaseService>();

        public ServiceManager()
        {
            AddService<ICharacterService>(new CharacterService());
            AddService<IDiceService>(new DiceService());
            AddService<IFightService>(new TurnBasedFightService());
            AddService<IMusicService>(new MusicService());
            AddService<ISheetService>(new SheetService());
            AddService<IWorldBuildingService>(new WorldBuildingService());
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