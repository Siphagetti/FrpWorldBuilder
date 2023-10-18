using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Prefab
{
    internal interface IPrefabService : Services.IBaseService
    {
        public Task<GameObject[]> LoadPrefabsInAssetBundlesForCategory(string categoryName);
        public IEnumerable<string> GetCategories();
        public Task<GameObject[]> ImportAssetBundle(string category);
    }
}
