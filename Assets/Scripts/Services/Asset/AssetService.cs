using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Asset
{
    internal class AssetService : IAseetService
    {
        private static readonly string _folderPath = Path.Combine(Path.Combine(Application.dataPath, "Resources"), "Assets");

        public List<GameObject> GetPrefabsInFolder(string relativeFolderPath)
        {
            /*
                Loads files that can be GameObject first.
                Then, search subfolders for the files that can be GameObject to load them.

                * relativeFolderPath is relative to '_folderPath', so does not contain '_subfolderName'
            */

            List<GameObject> list = new();

            string folderPath = Path.Combine(_folderPath, relativeFolderPath);

            if(!Directory.Exists(folderPath)) { Log.Logger.Log_Error("category_folder_not_found", relativeFolderPath); return null; }

            LoadPrefabs(folderPath, ref list);

            return list;
        }

        private void LoadPrefabs(string folderPath, ref List<GameObject> list)
        {
            Object[] loadedObjects = Resources.LoadAll(folderPath);

            foreach (Object obj in loadedObjects)
            {
                if (obj is GameObject)
                {
                    GameObject prefab = (GameObject)obj;
                    list.Add(prefab);
                }
            }
        }

        // Gets folders at Resources/Assets and get their names as category
        public List<string> GetAllCategories()
        {
            List<string> categories = new();

            var categoryFolders = Directory.GetDirectories(_folderPath, "*", SearchOption.TopDirectoryOnly);

            foreach (string categoryFolder in categoryFolders)
                categories.Add(Path.GetFileName(categoryFolder));

            return categories;
        }
    }
}
