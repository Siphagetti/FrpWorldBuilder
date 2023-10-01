using Save;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace World
{

    [Serializable]
    public struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(Quaternion quaternion)
        {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }

        // Overload the assignment operator (=) to assign from Quaternion
    public static implicit operator SerializableQuaternion(Quaternion q)
    {
        return new SerializableQuaternion(q);
    }

    // Overload the assignment operator (=) to assign to Quaternion
    public static implicit operator Quaternion(SerializableQuaternion sq)
    {
        return sq.ToQuaternion();
    }
    }

    [Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        // Overload the assignment operator (=) to assign from Vector3
        public static implicit operator SerializableVector3(Vector3 v)
        {
            return new SerializableVector3(v);
        }

        // Overload the assignment operator (=) to assign to Vector3
        public static implicit operator Vector3(SerializableVector3 sv)
        {
            return sv.ToVector3();
        }
    }

    [Serializable]
    public struct SerializableTransform
    {
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
        public SerializableVector3 scale;
    }

    [Serializable]
    struct GameObjectData
    {
        public string assetPath;
        public SerializableTransform transform;
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

namespace World
{
    // Keeps the world and controls builder and game session.
    public class WorldService : IWorldService
    {
        private readonly WorldsData worldsData = new();

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
                transform = new()
                {
                    position = new Vector3(0, 0, 0),
                    rotation = new Quaternion(0, 0, 0, 0),
                    scale = new Vector3(1, 1, 1),
                }
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
