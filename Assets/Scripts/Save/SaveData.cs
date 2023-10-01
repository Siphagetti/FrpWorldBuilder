using System;

namespace Save
{
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
}
