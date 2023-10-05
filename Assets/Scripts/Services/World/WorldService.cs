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
            Keeps the game object's data in the world. 

            When user add a gameobject to the world, a new 'GameObjectData' is created to be saved.
        */

        // The path that prefab of the gameobject loaded form Resources.
        public string assetPath;

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

        public void SaveWorld(string worldName)
        {
            /*
                Saves currently opened world's data to its own json file.
            */

            // -------- Create Json Data --------
            string jsonString = JsonUtility.ToJson(new GameObjectsData() { gameObjects = gameObjects });
            gameObjects.Clear(); // Clear the save list for future use.

            // -------- Create Folder --------
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
                Debug.Log($"Folder '{_folderPath}' has been created.");
            }

            // -------- Create File --------
            string fileName = worldName + ".json";
            string filePath = Path.Combine(_folderPath, fileName);
            File.WriteAllText(filePath, jsonString);
        }

        public Response LoadWorld(string worldName)
        {
            /*
                If the given world name's json file exists loads its data.
            */

            string fileName = worldName + ".json";
            string filePath = Path.Combine(_folderPath, fileName);

            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                GameObjectsData gameObjectsData = JsonUtility.FromJson<GameObjectsData>(jsonString);
                gameObjects = gameObjectsData.gameObjects;
                return new Response() { Success = true };
            }
            return new Response() { Success = false };
        }

        protected override Task Save()
        {
            base.Save();
            SaveWorld(currentWorldName);
            return Task.CompletedTask;
        }

        protected override Task Load(ref List<SaveData> data)
        {
            base.Load(ref data);
            LoadWorld(currentWorldName);
            return Task.CompletedTask;
        }
        public WorldsData() : base(key: "Worlds") { }
    }

    // Keeps the world and controls builder and game session.
    public class WorldService : IWorldService
    {
        private WorldsData worldsData = new();

        #region WorldManagement

        public Response CreateNewWorld(string worldName)
        {
            if (WorldExists(worldName)) return new Response { Success = false };

            SceneManager.CreateScene(worldName);
            SceneManager.LoadSceneAsync(worldName);
            worldsData.worlds.Add(new()
            {
                name = worldName,
                dataFile = worldName + ".json"
            });
            worldsData.SaveWorld(worldName);

            return new Response { Success = true };
        }
        public Response ChangeWorld(string worldName)
        {
            if (!WorldExists(worldName)) return new Response { Success = false };

            SceneManager.LoadSceneAsync(worldName);
            worldsData.LoadWorld(worldName);

            return new Response { Success = true };
        }
        
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
