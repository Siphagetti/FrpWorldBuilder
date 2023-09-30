using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Save
{
    // Temporary
    public class TestSavable : ASavable
    {
        public string id;
        public string data;
        public TestSavable(string key = "TestSavable", string keyModifier = "") : base(key, keyModifier) { }
    }
    //

    [Serializable]
    public struct SaveData
    {
        public string key; // Key to access service's data in the json file.
        public string data; // Data that converted to json

        public SaveData(string key, string data)
        {
            this.key = key;
            this.data = data;
        }
    }

    public abstract class ASavable : IEquatable<ASavable>
    {
        private readonly string _key;

        // Adds a json data for being saved to saveRepo list in the SaveManager.
        public Task Save() 
        {
            if (_key == "")
            {
                Debug.LogError(this + " has empty id for saving!");
                return Task.CompletedTask;
            }

            string data = JsonUtility.ToJson(this);
            if (data == "")
            {
                Debug.LogError(this + " has empty data to save!");
                return Task.CompletedTask;
            }

            SaveManager.Instance.AddSavedData(new(_key, data));
            Debug.Log(_key + " is added repository to be saved.");

            return Task.CompletedTask;
        }

        // Loads releated data in the save json via SaveManager.
        public Task Load(ref List<SaveData> data) 
        {
            SaveData saveData = data.FirstOrDefault(x => x.key == _key);

            if (EqualityComparer<SaveData>.Default.Equals(saveData, default))
            {
                Debug.LogError(_key + " has no saved data!");
                return Task.CompletedTask;
            }

            JsonUtility.FromJsonOverwrite(saveData.data, this);
            return Task.CompletedTask; 
        }

        protected ASavable(string key = "", string keyModifier = "") // A key modifier to handle different data for same class's objects.
        {
            SaveManager.Instance.RegisterSavable(this);
            _key = key + (keyModifier == "" ? "" : "_" + keyModifier);
        }

        ~ASavable() { SaveManager.Instance.UnregisterSavable(this); }

        public bool Equals(ASavable other)
        {
            if (other == null) return false;
            return string.Equals(_key, other._key, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() { return _key.GetHashCode(StringComparison.OrdinalIgnoreCase); }
    }

    public class SaveManager
    {
        private readonly string _folderPath = Path.Combine(Application.dataPath, "Save");

        #region Savable Repository

        private List<ASavable> savables = new List<ASavable>();

        public void RegisterSavable(ASavable savable)
        {
            if (savables.Contains(savable))
            {
                UnityEngine.Debug.Log(savable + " is already registered!");
                return;
            }
            savables.Add(savable);
        }

        public void UnregisterSavable(ASavable savable) { savables.Remove(savable); }

        #endregion

        #region Save

        // while saving, the created data will be kept by this list.
        private readonly List<SaveData> saveRepo = new();

        // while saving, the services access the save repo via this function.
        public void AddSavedData(SaveData data) { lock (saveRepo) { saveRepo.Add(data); } }

        private async Task SaveAll()
        {
            var loadTasks = savables.Select(m => m.Save());
            await Task.WhenAll(loadTasks);
        }

        public async void CreateSaveFile(string saveName)
        {
            await SaveAll();

            // -------- Create Json Data --------
            string jsonString = "{ \"dataList\": ["; // Start of JSON array

            for (int i = 0; i < saveRepo.Count; i++)
            {
                jsonString += JsonUtility.ToJson(saveRepo[i]);
                if (i < saveRepo.Count - 1) jsonString += ",";
            }
            jsonString += "] }"; // End of JSON array
            saveRepo.Clear(); // Clear the save list for future use.

            // -------- File --------
            string fileName = saveName + ".json";
            string filePath = Path.Combine(_folderPath, fileName);
            File.WriteAllText(filePath, jsonString);
        }

        #endregion

        #region Load

        private async Task LoadAll(List<SaveData> data)
        {
            var loadTasks = savables.Select(m => m.Load(ref data));
            await Task.WhenAll(loadTasks);
        }

        private class RootObject { public List<SaveData> dataList; } // requires for reading json.
        public async void LoadSaveFile(string saveName)
        {
            string fileName = saveName + ".json";
            string filePath = Path.Combine(_folderPath, fileName);
            
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);

                RootObject root = JsonUtility.FromJson<RootObject>(jsonString);
                await LoadAll(root.dataList);
            } else Debug.LogError("No file named " + saveName);
        }

        #endregion

        public static SaveManager Instance { get; private set; }
        public SaveManager()
        {
            if (Instance != null) return;
            Instance = this;

            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
                Debug.Log($"Folder '{_folderPath}' has been created.");
            }

            // Temporary
            TestSavable test = new TestSavable();
            TestSavable test1 = new TestSavable(keyModifier: "1");
            TestSavable test2 = new TestSavable(keyModifier: "2");
            LoadSaveFile("TestSave");
            //
        }
        ~SaveManager() 
        {
            savables.Clear();
            saveRepo.Clear();
        }
    }
}
