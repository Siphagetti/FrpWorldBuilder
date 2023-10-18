using SFB;
using System;
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

        public PrefabService()
        {
            _subFolderName = "AssetBundles";
            _rootFolderPath = Path.Combine(Application.streamingAssetsPath, _subFolderName);

            if (!Directory.Exists(_rootFolderPath)) Directory.CreateDirectory(_rootFolderPath);
        }

        public IEnumerable<string> GetCategories()
        {
            /*
                Categories are subfolders of the root folder. 
            */

            IEnumerable<string> categories = Directory.GetDirectories(_rootFolderPath, "*", SearchOption.TopDirectoryOnly).Select(c => Path.GetFileName(c));
            return categories;
        }

        public async Task<GameObject[]> ImportAssetBundle(string category)
        {
            var extensionList = new[] {
                new ExtensionFilter("Unity Asset Bundle", "*")
            };

            // Get asset bundles to be imported.
            var assetBundlePaths = StandaloneFileBrowser.OpenFilePanel("Select Asset Bundle", "", extensionList, false);

            if (assetBundlePaths == null || assetBundlePaths.Length == 0) return null;

            var assetBundlePath = assetBundlePaths[0];

            var categoryFolderPath = Path.Combine(_rootFolderPath, category);

            var fileName = Path.GetFileName(assetBundlePath);
            // If there is file that has the same name with the importing asset bundle cancel import and return null.
            if (File.Exists(Path.Combine(categoryFolderPath, fileName)))
            {
                Log.Logger.Log_Error("asset_bundle_exists", fileName, category);
                return null;
            }

            var prefabs = await LoadAssetBundle(assetBundlePath);

            // Copy the asset bundle into the category folder.
            if (prefabs != null) File.Copy(assetBundlePath, Path.Combine(categoryFolderPath, fileName));

            return prefabs;
        }

        public async Task<GameObject[]> LoadPrefabsInAssetBundlesForCategory(string category)
        {
            // Get category folder path
            var categoryFolderPath = Path.Combine(_rootFolderPath, category);

            // Get all asset bundles paths.
            string[] assetBundles = Directory.GetFiles(categoryFolderPath, "*.", SearchOption.TopDirectoryOnly);

            // For each asset bundle path call 'LoadAssetBundle' function and get each result as list.
            var loadBundles = assetBundles.Select(async bundlePath =>
            {
                try
                {
                    GameObject[] prefabs = await LoadAssetBundle(bundlePath);
                    return prefabs;
                }
                catch (Exception e) { Log.Logger.Log_Error("asset_bundle_load_error", e.Message); return new GameObject[0]; }
            }).ToList();

            List<GameObject> allPrefabs = new();
            
            // Get together all loaded prefabs in the category's folder.
            foreach (var loadBundle in loadBundles) allPrefabs.AddRange(await loadBundle);

            return allPrefabs.ToArray();
        }


        private async Task<GameObject[]> LoadAssetBundle(string bundlePath)
        {
            /*
                Loads prefabs in an asset bundle.
            */

            var tcs = new TaskCompletionSource<GameObject[]>();

            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);

            request.completed += operation =>
            {

                AssetBundle assetBundle = request.assetBundle;
                if (assetBundle == null)
                {
                    Log.Logger.Log_Error("asset_bundle_loadled_already", bundlePath);
                    tcs.SetResult(null);
                    return;
                }

                var prefabs = assetBundle.LoadAllAssets<GameObject>();

                prefabs.ToList().ForEach(p =>
                {
                    Prefab prefabComponent = p.AddComponent<Prefab>();
                    prefabComponent.Initialize();
                });
                tcs.SetResult(prefabs);

            };

            // Handle any errors or timeouts
            request.completed += operation =>
            {
                if (request.isDone && request.assetBundle == null) 
                    tcs.SetException(new Exception("Failed to load asset bundle."));
            };

            return await tcs.Task;
        }
    }
}
