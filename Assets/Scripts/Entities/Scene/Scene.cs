using Prefab;
using Save;
using Services;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hierarchy
{
    internal class Scene
    {
        // Constants for the folder path
        public readonly static string folderPath = Path.Combine(SaveManager.Instance._folderPath, "Scenes");

        private readonly string _jsonFilePath;

        // Private field for the scene's root GameObject
        private GameObject root;

        // Lists to store prefab data and the hierarchy structure
        public List<Prefab.Prefab> PrefabList { get; private set; } = new List<Prefab.Prefab>();

        [SerializeField] private List<string> hierarchyGroups = new();

        [SerializeField] private List<PrefabDTO> prefabs = new List<PrefabDTO>();

        public Scene(string sceneName)
        {
            // Initialize the JSON file path and create a new root GameObject for the scene
            _jsonFilePath = Path.Combine(folderPath, sceneName + ".json");
            root = new GameObject();

            // Reset the hierarchy when creating a new scene
            UnityEngine.Object.FindFirstObjectByType<HierarchyManager>().ResetHierarchy();

            Load();
        }

        // Add a prefab to the scene
        public void AddPrefab(Prefab.Prefab prefab)
        {
            prefab.transform.SetParent(root.transform);
            PrefabList.Add(prefab);
            prefabs.Add(prefab.Data);
        }

        // Delete a prefab from the scene
        public void DeletePrefab(Prefab.Prefab prefab)
        {
            PrefabList.Remove(prefab);
            prefabs.Remove(prefab.Data);
            UnityEngine.Object.Destroy(prefab.gameObject);
        }

        // Close and clean up the scene
        public void CloseScene()
        {
            UnityEngine.Object.Destroy(root);
        }

        // Check if the JSON save file for the scene exists
        public bool IsSaveFileExists() => File.Exists(_jsonFilePath);

        // Delete the JSON save file for the scene
        public void Delete()
        {
            if (File.Exists(_jsonFilePath))
                File.Delete(_jsonFilePath);
        }

        public void AddHierarchyGroup(string groupName)
        {
            if (!hierarchyGroups.Contains(groupName))
            {
                hierarchyGroups.Add(groupName);
            }
        }

        public void DeleteHierarchyGroup(string groupName)
        {
            hierarchyGroups.Remove(groupName);
        }

        #region Save/Load

        // Save the scene to a JSON file
        public void Save()
        {
            foreach (var prefab in PrefabList)
                prefab.UpdateTransform();

            // Convert the scene object to a JSON string and write it to a file
            string jsonString = JsonUtility.ToJson(this);
            File.WriteAllText(_jsonFilePath, jsonString);
        }

        // Load the scene from a JSON file
        private void Load()
        {
            if (File.Exists(_jsonFilePath))
            {
                // Read the JSON file and deserialize it to populate the scene object
                string jsonString = File.ReadAllText(_jsonFilePath);
                JsonUtility.FromJsonOverwrite(jsonString, this);

                // Load hierarchy gruops in the scene
                foreach (var group in hierarchyGroups)
                {
                    UnityEngine.Object.FindFirstObjectByType<HierarchyManager>().CreateGroup(group);
                }

                // Load the prefabs into the scene from the hierarchy data
                PrefabList = ServiceManager.GetService<IPrefabService>().LoadPrefabs(root.transform, prefabs);
            }
        }

        #endregion
    }
}
