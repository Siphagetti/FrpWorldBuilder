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
        private readonly string _subFolderName;
        private readonly string _rootFolderPath;

        private PrefabRepository _prefabRepo;

        public PrefabRepository GetRepository() => _prefabRepo;

        public PrefabService()
        {
            _subFolderName = "AssetBundles";
            _rootFolderPath = Path.Combine(Application.streamingAssetsPath, _subFolderName);

            if (!Directory.Exists(_rootFolderPath)) Directory.CreateDirectory(_rootFolderPath);

            _prefabRepo = new PrefabRepository(GetCategories());

            LoadAllPrefabs();
        }

        public IEnumerable<string> GetCategories()
        {
            /*
                Categories are subfolders of the root folder. 
            */

            IEnumerable<string> categories = Directory.GetDirectories(_rootFolderPath, "*", SearchOption.TopDirectoryOnly).Select(c => Path.GetFileName(c));
            return categories;
        }

        public async Task<Response<(string, Prefab[])>> ImportAssetBundle(string category)
        {
            var extensionList = new[] { new ExtensionFilter("File", "*") };

            // Get asset bundles to be imported.
            var assetBundlePaths = StandaloneFileBrowser.OpenFilePanel("Select Asset Bundle", "", extensionList, false);

            if (assetBundlePaths == null || assetBundlePaths.Length == 0) return new Response<(string, Prefab[])>() { Success = false };

            var assetBundlePath = assetBundlePaths[0];

            var categoryFolderPath = Path.Combine(_rootFolderPath, category);

            var fileName = Path.GetFileName(assetBundlePath);

            // If there is file that has the same name with the importing asset bundle cancel import and return null.
            if (File.Exists(Path.Combine(categoryFolderPath, fileName)))
            {
                Log.Logger.Log_Error("asset_bundle_exists", fileName, category);
                return new Response<(string, Prefab[])>() { Success = false };
            }

            // Response has asset bundle name as result.
            var response = await LoadAssetBundle(category, assetBundlePath);

            if (!response.Success) return new Response<(string, Prefab[])>() { Success = false };

            File.Copy(assetBundlePath, Path.Combine(categoryFolderPath, fileName));

            var prefabs = _prefabRepo.GetPrefabs(category, response.Result);

            return new Response<(string, Prefab[])>() { Success = true, Result = (response.Result, prefabs) };
        }

        public void DeleteCategory(string category)
        {
            _prefabRepo.RemoveCategory(category);

            var folderPath = Path.Combine(_rootFolderPath, category);
            Directory.Delete(folderPath, true);
        }

        public void DeleteAssetBundle(string category, string bundleName)
        {
            _prefabRepo.RemoveAssetBundle(category, bundleName);

            var filePath = Path.Combine(Path.Combine(_rootFolderPath, category), bundleName);
            File.Delete(filePath);
        }

        private async void LoadAllPrefabs()
        {
            List<Task> loadTasks = new();
            
            foreach (var category in GetCategories()) loadTasks.Add(LoadAssetBundlesInCategory(category));

            await Task.WhenAll(loadTasks);
        }

        private async Task LoadAssetBundlesInCategory(string category)
        {
            // Get category folder path
            var categoryFolderPath = Path.Combine(_rootFolderPath, category);

            // Get all asset bundles paths.
            string[] bundlePaths = Directory.GetFiles(categoryFolderPath, "*.", SearchOption.TopDirectoryOnly);

            List<Task> loadBundles = new();
            foreach (var bundlePath in bundlePaths) loadBundles.Add(LoadAssetBundle(category, bundlePath));

            await Task.WhenAll(loadBundles);
        }

        private async Task<Response<string>> LoadAssetBundle(string category, string bundlePath)
        {
            /*
                Loads prefabs in an asset bundle.
                Returns asset bundle name.
            */


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
                bool success = prefabComponent.Initialize(p.name, bundle.name, bundlePath);
                if (success) readyPrefabs.Add(prefabComponent);
            });

            _prefabRepo.AddAssetBundle(category, bundle.name, readyPrefabs.ToArray());
            return await Task.FromResult(new Response<string>() { Success = true, Result = bundle.name });
        }
    }
}
