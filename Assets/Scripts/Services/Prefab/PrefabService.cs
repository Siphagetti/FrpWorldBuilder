using Hierarchy;
using SFB;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Prefab
{
    internal class PrefabService : IPrefabService
    {
        // Fields for the subfolder name and root folder path
        private readonly string subFolderName;
        private readonly string rootFolderPath;

        private PrefabRepository prefabRepo;

        public PrefabRepository GetRepository() => prefabRepo;

        public PrefabService()
        {
            subFolderName = "AssetBundles";
            rootFolderPath = Path.Combine(Application.streamingAssetsPath, subFolderName);

            // Ensure the root folder exists
            if (!Directory.Exists(rootFolderPath))
            {
                Directory.CreateDirectory(rootFolderPath);
            }

            prefabRepo = new PrefabRepository(GetCategories());

            LoadAllPrefabs();
        }

        // Get a list of category names from subfolders
        public IEnumerable<string> GetCategories()
        {
            IEnumerable<string> categories = Directory.GetDirectories(rootFolderPath, "*", SearchOption.TopDirectoryOnly)
                .Select(c => Path.GetFileName(c));
            return categories;
        }

        // Import an asset bundle and add prefabs to the repository
        public async Task<Response<(string, Prefab[])>> ImportAssetBundle(string category)
        {
            var extensionList = new[] { new ExtensionFilter("File", "*") };

            var assetBundlePaths = StandaloneFileBrowser.OpenFilePanel("Select Asset Bundle", "", extensionList, false);

            if (assetBundlePaths == null || assetBundlePaths.Length == 0)
            {
                return new Response<(string, Prefab[])>() { Success = false };
            }

            var assetBundlePath = assetBundlePaths[0];
            var categoryFolderPath = Path.Combine(rootFolderPath, category);
            var fileName = Path.GetFileName(assetBundlePath);

            if (File.Exists(Path.Combine(categoryFolderPath, fileName)))
            {
                Log.Logger.Log_Error("asset_bundle_exists", fileName, category);
                return new Response<(string, Prefab[])>() { Success = false };
            }

            var response = await LoadAssetBundle(category, assetBundlePath);

            if (!response.Success)
            {
                return new Response<(string, Prefab[])>() { Success = false };
            }

            File.Copy(assetBundlePath, Path.Combine(categoryFolderPath, fileName));

            var prefabs = prefabRepo.GetPrefabs(category, response.Result);

            return new Response<(string, Prefab[])>() { Success = true, Result = (response.Result, prefabs) };
        }

        // Delete a category and its associated asset bundles
        public void DeleteCategory(string category)
        {
            prefabRepo.RemoveCategory(category);

            var folderPath = Path.Combine(rootFolderPath, category);
            Directory.Delete(folderPath, true);
        }

        // Delete an asset bundle and its associated prefabs
        public void DeleteAssetBundle(string category, string bundleName)
        {
            prefabRepo.RemoveAssetBundle(category, bundleName);

            var filePath = Path.Combine(Path.Combine(rootFolderPath, category), bundleName);
            File.Delete(filePath);
        }

        // Load all prefabs from asset bundles
        private async void LoadAllPrefabs()
        {
            List<Task> loadTasks = new List<Task>();

            foreach (var category in GetCategories())
            {
                loadTasks.Add(LoadAssetBundlesInCategory(category));
            }

            await Task.WhenAll(loadTasks);
        }

        // Load asset bundles in a category
        private async Task LoadAssetBundlesInCategory(string category)
        {
            var categoryFolderPath = Path.Combine(rootFolderPath, category);
            string[] bundlePaths = Directory.GetFiles(categoryFolderPath, "*.", SearchOption.TopDirectoryOnly);

            List<Task> loadBundles = new List<Task>();

            foreach (var bundlePath in bundlePaths)
            {
                loadBundles.Add(LoadAssetBundle(category, bundlePath));
            }

            await Task.WhenAll(loadBundles);
        }

        // Load assets from an asset bundle and add them to the repository
        private async Task<Response<string>> LoadAssetBundle(string category, string bundlePath)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);

            if (bundle == null)
            {
                Log.Logger.Log_Error("asset_bundle_load_error", bundlePath);
                return await Task.FromResult(new Response<string>() { Success = false });
            }

            var prefabs = bundle.LoadAllAssets<GameObject>();
            var readyPrefabs = new List<Prefab>();

            prefabs.ToList().ForEach(p =>
            {
                Prefab prefabComponent = p.AddComponent<Prefab>();
                bool success = prefabComponent.Initialize(new PrefabDTO { prefabName = p.name, assetBundleName = bundle.name }, bundlePath);
                if (success) readyPrefabs.Add(prefabComponent);
            });

            prefabRepo.AddCategory(category);
            prefabRepo.AddAssetBundle(category, bundle.name, readyPrefabs.ToArray());

            string bundleName = bundle.name;
            bundle.Unload(false);

            return await Task.FromResult(new Response<string>() { Success = true, Result = bundleName });
        }

        // Create a new category folder
        public void NewCategory(string categoryName)
        {
            var newCategoryPath = Path.Combine(rootFolderPath, categoryName);
            Directory.CreateDirectory(newCategoryPath);
        }

        // Load prefabs into the scene based on prefab data
        public List<Prefab> LoadPrefabs(Transform parent, List<PrefabDTO> prefabsData)
        {
            var hierarchyManager = Object.FindFirstObjectByType<HierarchyManager>();
            List<Prefab> prefabs = new List<Prefab>();

            foreach (var prefabData in prefabsData)
            {
                var prefab = prefabRepo.GetPrefab(prefabData.assetBundleName, prefabData.prefabName);

                var wrapper = prefab.CreateWrapper(prefabData.transform.position);
                prefab = wrapper.GetComponent<Prefab>();
                prefab.Data = prefabData;
                prefabs.Add(prefab);

                wrapper.transform.rotation = prefabData.transform.rotation;
                wrapper.transform.localScale = prefabData.transform.localScale;
                wrapper.transform.SetParent(parent, true);

                hierarchyManager.GetComponent<HierarchyManager>().LoadHierarchyElement(prefab, prefabData.hierarchyGroupName);
            }

            return prefabs;
        }
    }
}
