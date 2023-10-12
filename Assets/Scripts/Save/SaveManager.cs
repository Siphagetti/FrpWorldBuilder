using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Save
{
    public class SaveManager
    {
        public readonly string _folderPath = Path.Combine(Application.streamingAssetsPath, "Save");

        private string _currentSaveFileName = "Save.json";

        #region Savable Repository

        private List<SavableObject> savables = new List<SavableObject>();

        public void RegisterSavable(SavableObject savable, SaveDelegate save, LoadDelegate load)
        {
            if (savables.Contains(savable)) return;
            
            savables.Add(savable);
            saveDelegates.Add(save);
            loadDelegates.Add(load);
        }
        public void UnregisterSavable(SavableObject savable, SaveDelegate save, LoadDelegate load) 
        { 
            savables.Remove(savable);
            saveDelegates.Remove(save);
            loadDelegates.Remove(load);
        }

        #endregion

        #region Save

        // Delegate for the Save method
        public delegate Task SaveDelegate();

        // Lists to store the delegates
        private List<SaveDelegate> saveDelegates = new();

        // while saving, the services access the save repo via this function.
        public void AddSavedData(SaveData data) { lock (_root.dataList) { _root.dataList.Add(data); } }

        private async Task SaveAll()
        {
            var loadTasks = saveDelegates.Select(s => s.Invoke());
            await Task.WhenAll(loadTasks);
        }

        public async void Save(string saveName = "")
        {
            await SaveAll();

            // -------- Create Json Data --------
            string jsonString = JsonUtility.ToJson(_root);
            _root.dataList.Clear(); // Clear the save list for future use.

            // -------- Create Folder --------
            if (!Directory.Exists(_folderPath)) Directory.CreateDirectory(_folderPath);

            // -------- File --------
            string filePath = Path.Combine(_folderPath, _currentSaveFileName = saveName == "" ? _currentSaveFileName : saveName + ".json");
            File.WriteAllText(filePath, jsonString);
            Log.Logger.Log_Info("save_success");
        }

        #endregion

        #region Load

        // Delegate for the Load method
        public delegate Task LoadDelegate(ref List<SaveData> data);

        private List<LoadDelegate> loadDelegates = new();

        private async Task LoadAll(List<SaveData> data)
        {
            var loadTasks = loadDelegates.Select(l => l.Invoke(ref data));
            await Task.WhenAll(loadTasks);
        }

        public async void Load(string saveName = "")
        {
            _currentSaveFileName = saveName == "" ? _currentSaveFileName : saveName + ".json";

            string filePath = Path.Combine(_folderPath, _currentSaveFileName);
            
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);

                RootObject root = JsonUtility.FromJson<RootObject>(jsonString);
                await LoadAll(root.dataList);
            }
        }

        #endregion

        public static SaveManager Instance { get; private set; }
        public SaveManager()
        {
            if (Instance != null) return;
            Instance = this;
        }
        ~SaveManager() 
        {
            savables.Clear();
            _root.dataList.Clear();
        }

        private class RootObject // requires for reading json.
        {
            // while saving, the created data will be kept by this list.
            public List<SaveData> dataList = new();
        }

        private readonly RootObject _root = new();
    }
}
