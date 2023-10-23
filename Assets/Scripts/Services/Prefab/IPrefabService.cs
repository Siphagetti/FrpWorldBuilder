using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prefab
{
    internal interface IPrefabService : Services.IBaseService
    {
        public PrefabRepository GetRepository(); 
        public IEnumerable<string> GetCategories();
        public Task<Response<Prefab[]>> ImportAssetBundle(string category);
    }
}
