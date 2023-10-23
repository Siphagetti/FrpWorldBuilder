using Save;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace World
{
    [Serializable]
    struct GameObjectData
    {
        /*
            Keeps the game object's transform data. 

            When user add a gameobject to the world, a new 'GameObjectData' is created to be saved.
        */

        // The path that prefab of the gameobject loaded form Resources.
        public readonly string assetBundlePath;
        public readonly string category;
        public readonly string name;

        // Transform of the gameobject in the world.
        public SerializableTransform transform;

        
    }

    [Serializable]
    struct GameObjectsData
    {
        /*
            A tool to convert json data to List<GameObjectData>
        */

        public List<GameObjectData> gameObjects;
    }

    [Serializable]
    struct WorldData
    {
        /*
            All created worlds data files are seperated. 
            This helps to keep and find these files' addresses in the main save file.
        */

        // Name of the world
        public string name;

        // Json file name that keeps worlds data.
        public string dataFile;
    }

    class WorldsData : SavableObject
    {
        [NonSerialized]
        private readonly string _folderPath = Path.Combine(SaveManager.Instance._folderPath, "Worlds");

        [NonSerialized]
        public List<GameObjectData> gameObjects = new();

        // Data to be saved
        public string currentWorldName;
        public List<WorldData> worlds = new();

        public void SaveWorld()
        {
            /*
                Saves currently opened world's data to its own json file.
            */

            // -------- Create Json Data --------
            string jsonString = JsonUtility.ToJson(new GameObjectsData() { gameObjects = gameObjects });
            gameObjects.Clear(); // Clear the save list for future use.

            // -------- Create Folder --------
            if (!Directory.Exists(_folderPath)) Directory.CreateDirectory(_folderPath);

            // -------- Create File --------
            string fileName = currentWorldName + ".json";
            string filePath = Path.Combine(_folderPath, fileName);
            File.WriteAllText(filePath, jsonString);
        }

        public void LoadWorld()
        {
            /*
                If the given world name's json file exists loads its data.
            */

            string fileName = currentWorldName + ".json";
            string filePath = Path.Combine(_folderPath, fileName);

            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                GameObjectsData gameObjectsData = JsonUtility.FromJson<GameObjectsData>(jsonString);
                gameObjects = gameObjectsData.gameObjects;
            }
        }

        protected override Task Save()
        {
            base.Save();
            SaveWorld();
            return Task.CompletedTask;
        }

        protected override Task Load(ref List<SaveData> data)
        {
            base.Load(ref data);
            LoadWorld();
            return Task.CompletedTask;
        }
        public WorldsData() : base(key: "Worlds") { }
    }

    // Keeps the world and controls builder and game session.
    public class WorldService : IWorldService
    {
        private WorldsData worldsData = new();

        #region WorldManagement
        
        public void SaveWorld() => worldsData.SaveWorld();
        public void LoadWorld() => worldsData.LoadWorld();

        #endregion

        #region Helper Functions

        // Helper function to check if a scene exists by name.
        private bool WorldExists(string worldName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == worldName) return true;
            }
            return false;
        }

        #endregion
    }
}
