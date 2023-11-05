using Prefab;

namespace Hierarchy
{
    internal interface ISceneService : Services.IBaseService
    {
        public void AddHierarchyGroupToCurrentScene(string groupName);
        public void DeleteHierarchyGroupToCurrentScene(string groupName);
        public void AddPrefabToCurrentScene(Prefab.Prefab prefabData);
        public void AddScene(string sceneName);
        public void DeleteScene(string sceneName);
        public void DeletePrefab(Prefab.Prefab prefab);
        public void DeletePrefabs(Prefab.Prefab[] prefabsToDelete);

    }
}
