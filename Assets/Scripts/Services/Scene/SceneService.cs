using Save;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Hierarchy
{
    internal class SceneService : SavableObject, ISceneService
    {
        // Serialized fields for the current scene name and a list of scene names
        [SerializeField] private string currentSceneName = "defaultScene";
        [SerializeField] private List<string> sceneNames = new List<string>();

        // Property to access the current scene
        public Scene CurrentScene { get; private set; }

        public SceneService() : base(key: "Scenes")
        {
            // Ensure the scenes folder exists
            if (!Directory.Exists(Scene.folderPath))
            {
                Directory.CreateDirectory(Scene.folderPath);
            }
        }

        // Add a prefab to the current scene
        public void AddPrefabToCurrentScene(Prefab.Prefab prefab)
        {
            CurrentScene.AddPrefab(prefab);
        }

        // Delete a prefab from the current scene
        public void DeletePrefab(Prefab.Prefab prefab)
        {
            CurrentScene.DeletePrefab(prefab);
        }

        // Delete an array of prefabs from the current scene
        public void DeletePrefabs(Prefab.Prefab[] prefabsToDelete)
        {
            int i = 0;

            foreach (var prefab in new List<Prefab.Prefab>(CurrentScene.PrefabList))
            {
                if (prefab == prefabsToDelete[i])
                {
                    CurrentScene.DeletePrefab(prefab);
                    i++;

                    if (i == prefabsToDelete.Length)
                    {
                        return;
                    }
                }
            }
        }

        // Add a new scene to the list of scenes
        public void AddScene(string sceneName)
        {
            if (!sceneNames.Contains(sceneName))
            {
                sceneNames.Add(sceneName);
            }
            else
            {
                Log.Logger.Log_Error("scene_exists", sceneName);
            }
        }

        // Delete a scene, including its current scene data
        public void DeleteScene(string sceneName)
        {
            if (sceneName == currentSceneName)
            {
                CurrentScene.CloseScene();
                CurrentScene.Delete();
            }
            else if (sceneNames.Contains(sceneName))
            {
                sceneNames.Remove(sceneName);
                new Scene(sceneName).Delete();
            }
        }

        // Change the current scene to the specified scene name
        public void ChangeScene(string sceneName)
        {
            if (sceneNames.Contains(sceneName))
            {
                CurrentScene.CloseScene();
                CurrentScene = new Scene(sceneName);
            }
        }

        #region Save/Load

        // Override the Save method to save the current scene
        protected override Task Save()
        {
            if (CurrentScene != null)
            {
                CurrentScene.Save();
            }
            return base.Save();
        }

        // Override the Load method to load the current scene and its data
        protected override Task Load(ref List<SaveData> data)
        {
            base.Load(ref data);

            CurrentScene = new Scene(currentSceneName);
            CurrentScene.Load();

            return Task.CompletedTask;
        }

        #endregion
    }
}
