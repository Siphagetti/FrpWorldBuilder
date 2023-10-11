using System.Collections.Generic;
using UserInterface.World.Building.Prefab;

namespace Prefab
{
    internal interface IPrefabService : Services.IBaseService
    {
        public PrefabEntity[] GetPrefabEntitiesInFolder(string folderPath);
        public List<string> GetAllCategories();
    }
}
