using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prefab
{
    internal interface IPrefabService : Services.IBaseService
    {
        public PrefabRepository GetRepository();
        public void DeleteCategory(string category);
        public void DeleteAssetBundle(string category, string bundleName);
        public IEnumerable<string> GetCategories();
        public Task<Response<(string, Prefab[])>> ImportAssetBundle(string category);
    }
}
