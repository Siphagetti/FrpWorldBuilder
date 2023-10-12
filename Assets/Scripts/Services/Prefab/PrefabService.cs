using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UserInterface.World.Building.Prefab;

namespace Prefab
{
    class Categories : Save.SavableObject
    {
        public List<string> categoryList = new();

        protected override Task Save()
        {
            #if UNITY_EDITOR
            
            /*
                If in unity editor, gets subfolders at Resources/Assets folder and get their names as category.
            */

            string folderPath = Path.Combine(Path.Combine(Application.dataPath, "Resources"), "Prefabs");
            var directories = Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly);

            List<string> categories = new List<string>();
            foreach (var directory in directories) categories.Add(Path.GetFileName(directory));
            categoryList = categories;

            #endif

            return base.Save();
        }

        public Categories() : base(key: "Categories") { }
    }
    internal class PrefabService : IPrefabService
    {
        private Categories _categories = new();

        public IEnumerable<PrefabEntity> GetPrefabEntitiesInFolder(string category)
        {
            List<PrefabEntity> prefabEntities = Resources.LoadAll<PrefabEntity>("").ToList();
            var category_prefabs = prefabEntities.Where(x => x.category == category);
            return category_prefabs;
        }

        // Gets folders at Resources/Assets and get their names as category
        public List<string> GetCategories() => new(_categories.categoryList);
    }
}
