namespace Prefab
{
    internal interface IPrefabService : Services.IBaseService
    {
        public void SavePrefab(string sourcePath, string relatedFolderPath = "");
        public UnityEngine.GameObject LoadPrefab(string name, string relatedFolderPath = "");
    }
}
