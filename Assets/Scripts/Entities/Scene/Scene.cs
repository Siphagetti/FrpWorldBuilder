using Prefab;
using Save;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hierarchy
{
    internal class Scene
    {
        [NonSerialized]
        public readonly static string _folderPath = Path.Combine(SaveManager.Instance._folderPath, "Scenes");

        [NonSerialized]
        private readonly string jsonFilePath;

        [NonSerialized] private GameObject _root;

        public List<Prefab.Prefab> PrefabList { get; private set; } = new();

        [SerializeField] private List<PrefabDTO> hierarchy = new();

        public void AddPrefab(Prefab.Prefab prefab)
        {
            prefab.transform.SetParent(_root.transform);
            PrefabList.Add(prefab);
            hierarchy.Add(prefab.Data);
        }

        public void DeletePrefab(Prefab.Prefab prefab)
        {
            PrefabList.Remove(prefab);
            hierarchy.Remove(prefab.Data);
            UnityEngine.Object.Destroy(prefab.gameObject);
        }

        public void CloseScene()
        {
            UnityEngine.Object.Destroy(_root);
        }

        public bool IsSaveFileExists() => File.Exists(jsonFilePath);
        public void Delete() { if (File.Exists(jsonFilePath)) File.Delete(jsonFilePath); }

        #region Save/Load

        public void Save()
        {
            string jsonString = JsonUtility.ToJson(this);
            File.WriteAllText(jsonFilePath, jsonString);
        }

        public void Load()
        {
            if (File.Exists(jsonFilePath))
            {
                string jsonString = File.ReadAllText(jsonFilePath);
                JsonUtility.FromJsonOverwrite(jsonString, this);
                PrefabList = ServiceManager.GetService<IPrefabService>().LoadPrefabs(_root.transform, hierarchy);
            }
        }

        #endregion

        public Scene(string sceneName)
        {
            jsonFilePath = Path.Combine(_folderPath, sceneName + ".json");
            _root = new GameObject();
        }
    }
}
