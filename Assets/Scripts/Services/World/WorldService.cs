using Save;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace World.Save
{
    [Serializable]
    struct GameObjectData
    {
        public string assetPath;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    class GameObjectsData
    {
        public List<GameObjectData> gameObjects;
    }

    [Serializable]
    struct WorldData
    {
        public string name;
        public string dataFile;
    }

    class WorldsData : ASavable
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
        public WorldsData() : base(key:"Worlds") { }
    }
}

namespace Services
{
    // Keeps the world and controls builder and game session.
    public class WorldService : IWorldService
    {
        private readonly World.Save.WorldsData worldsData = new();

        public WorldService()
        {
            // ------ Temporary ------
            worldsData.currentWorldName = "asd";
            worldsData.worlds.Add(new()
            {
                name = "asd",
                dataFile = "asd.json"
            });
            worldsData.gameObjects.Add(new()
            {
                assetPath = "asdfasd",
                position = new Vector3(0,0, 0),
                rotation = new Quaternion(0,0,0,0),
                scale = new Vector3(1,1,1),
            });
            SaveManager.Instance.CreateSaveFile("TestSave");
            //SaveManager.Instance.LoadSaveFile("TestSave");
            //Debug.Log(worldsData.gameObjects[0].assetPath);
            // ------------------------
        }
        #region WorldManagement

        public Response CreateNewWorld(string worldName)
        {
            if (WorldExists(worldName)) return new Response { Success = false };

            SceneManager.CreateScene(worldName);
            SceneManager.LoadScene(worldName);
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

            SceneManager.LoadScene(worldName);
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
