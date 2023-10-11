using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UserInterface.World.Building.Prefab;

namespace Prefab
{
    internal class PrefabService : IPrefabService
    {
        private static readonly string _subfolder = "Assets";
        private static readonly string _folderPath = Path.Combine(Path.Combine(Application.dataPath, "Resources"), _subfolder);

        public PrefabEntity[] GetPrefabEntitiesInFolder(string category)
        {
            string folderPath = Path.Combine(_folderPath, category);

            if(!Directory.Exists(folderPath)) { Log.Logger.Log_Error("category_folder_not_found", category); return null; }

            PrefabEntity[] prefabEntities = Resources.LoadAll<PrefabEntity>(Path.Combine(_subfolder, category)); ;

            return prefabEntities;
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
