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
        private List<PrefabDTO> hierarchy = new List<PrefabDTO>();

        public Scene(string sceneName)
        {
            // Initialize the JSON file path and create a new root GameObject for the scene
            _jsonFilePath = Path.Combine(folderPath, sceneName + ".json");
            root = new GameObject();

            // Reset the hierarchy when creating a new scene
            UnityEngine.Object.FindFirstObjectByType<HierarchyManager>().ResetHierarchy();
        }

        // Add a prefab to the scene
        public void AddPrefab(Prefab.Prefab prefab)
        {
            prefab.transform.SetParent(root.transform);
            PrefabList.Add(prefab);
            hierarchy.Add(prefab.Data);
        }

        // Delete a prefab from the scene
        public void DeletePrefab(Prefab.Prefab prefab)
        {
            PrefabList.Remove(prefab);
            hierarchy.Remove(prefab.Data);
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
        public void Load()
        {
            if (File.Exists(_jsonFilePath))
            {
                // Read the JSON file and deserialize it to populate the scene object
                string jsonString = File.ReadAllText(_jsonFilePath);
                JsonUtility.FromJsonOverwrite(jsonString, this);

                // Load the prefabs into the scene from the hierarchy data
                PrefabList = ServiceManager.GetService<IPrefabService>().LoadPrefabs(root.transform, hierarchy);
            }
        }

        #endregion
    }
}
