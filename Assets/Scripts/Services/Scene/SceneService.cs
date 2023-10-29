using Prefab;
using Save;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Hierarchy
{
    internal class SceneService : SavableObject, ISceneService
    {
        [SerializeField] private string currentSceneName = "defaultScene";

        [SerializeField] private List<string> sceneNames = new();

        public Scene CurrentScene { get; private set; }

        public SceneService() : base(key: "Scenes")
        {
            if (!Directory.Exists(Scene._folderPath)) Directory.CreateDirectory(Scene._folderPath);
        }

        public void AddPrefabToCurrentScene(Prefab.Prefab prefab)
        {
            CurrentScene.AddPrefab(prefab);
        }

        public void DeletePrefab(Prefab.Prefab prefab)
        {
            CurrentScene.DeletePrefab(prefab);
        }

        public void DeletePrefabs(Prefab.Prefab[] prefabsToDelete)
        {
            int i = 0;

            foreach (var prefab in new List<Prefab.Prefab>(CurrentScene.PrefabList))
            {
                if (prefab == prefabsToDelete[i])
                {
                    CurrentScene.DeletePrefab(prefab);
                    i++;

                    if (i == prefabsToDelete.Length) return;
                }
            }
        }

        public void AddScene(string sceneName)
        {
            if (!sceneNames.Contains(sceneName)) sceneNames.Add(sceneName);
            else Log.Logger.Log_Error("scene_exists", sceneName);
        }

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

        public void ChangeScene(string sceneName)
        {
            if (sceneNames.Contains(sceneName))
            {
                CurrentScene.CloseScene();
                CurrentScene = new Scene(sceneName);
            }
        }

        #region Save/Load

        protected override Task Save()
        {
            if (CurrentScene != null) CurrentScene.Save();
            return base.Save();
        }

        protected override Task Load(ref List<SaveData> data)
        {
            base.Load(ref data);

            CurrentScene = new(currentSceneName);
            CurrentScene.Load();

            return Task.CompletedTask;
        }

        #endregion

    }
}
