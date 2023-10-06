using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Save
{
    public abstract class SavableObject : IEquatable<SavableObject>
    {
        private readonly string _key;

        // Adds a json data for being saved to saveRepo list in the SaveManager.
        virtual protected Task Save()
        {
            if (_key == "") return Task.CompletedTask;

            string data = JsonUtility.ToJson(this);
            if (data == "") return Task.CompletedTask;

            SaveManager.Instance.AddSavedData(new(_key, data));
            return Task.CompletedTask;
        }

        // Loads releated data in the save json via SaveManager.
        virtual protected Task Load(ref List<SaveData> data)
        {
            SaveData saveData = data.FirstOrDefault(x => x.key == _key);

            if (EqualityComparer<SaveData>.Default.Equals(saveData, default)) return Task.CompletedTask;
            
            JsonUtility.FromJsonOverwrite(saveData.data, this);
            return Task.CompletedTask;
        }

        protected SavableObject(string key = "", string keyModifier = "") // A key modifier to handle different data for same class's objects.
        {
            SaveManager.Instance.RegisterSavable(this, Save, Load);
            _key = key + (keyModifier == "" ? "" : "_" + keyModifier);
        }

        ~SavableObject() { SaveManager.Instance.UnregisterSavable(this, Save, Load); }

        public bool Equals(SavableObject other)
        {
            if (other == null) return false;
            return string.Equals(_key, other._key, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() { return _key.GetHashCode(StringComparison.OrdinalIgnoreCase); }
    }
}
