using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Prefab
{
    internal interface IPrefabService : Services.IBaseService
    {
        public PrefabRepository GetRepository();
        public void NewCategory(string categoryName);
        public void DeleteCategory(string category);
        public void DeleteAssetBundle(string category, string bundleName);
        public IEnumerable<string> GetCategories();
        public Task<Response<(string, Prefab[])>> ImportAssetBundle(string category);
        public List<Prefab> LoadPrefabs(Transform parent, List<PrefabDTO> prefabsData);
    }
}
