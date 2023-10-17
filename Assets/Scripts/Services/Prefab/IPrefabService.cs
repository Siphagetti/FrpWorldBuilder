using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Prefab
{
    internal interface IPrefabService : Services.IBaseService
    {
        public Task<List<GameObject>> LoadPrefabsInAssetBundlesForCategory(string categoryName);
        public IEnumerable<string> GetCategories();
        public Task<List<GameObject>> ImportAssetBundle(string category);
    }
}
