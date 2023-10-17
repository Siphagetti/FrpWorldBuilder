using SFB;
using System;
using System.Collections;
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

        public async Task<List<GameObject>> ImportAssetBundle(string category)
        {
            var extensionList = new[] {
                new ExtensionFilter("UnityAssetBundle", "")
            };

            // Get asset bundles to be imported.
            var assetBundlePaths = StandaloneFileBrowser.OpenFilePanel("Select Asset Bundle", "", extensionList, true);

            if (assetBundlePaths == null || assetBundlePaths.Length == 0) return null;

            var categoryFolderPath = Path.Combine(_rootFolderPath, category);

            // For each asset bundle path call 'LoadAssetBundle' function and get each result as list.
            // Also copy the asset bundle into the category folder.
            var loadBundles = assetBundlePaths.Select(async bundlePath =>
            {
                try
                {
                    GameObject[] prefabs = await LoadAssetBundle(bundlePath);
                    Task.Run(() => File.Copy(bundlePath, categoryFolderPath));
                    return prefabs;
                }
                catch (Exception e) { Log.Logger.Log_Error("asset_bundle_load_error", e.Message); return new GameObject[0]; }
            }).ToList();

            List<GameObject> allPrefabs = new();

            // Get together all loaded prefabs in the category's folder.
            foreach (var loadBundle in loadBundles) allPrefabs.AddRange(await loadBundle);

            return allPrefabs;
        }

        public async Task<List<GameObject>> LoadPrefabsInAssetBundlesForCategory(string category)
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

            return allPrefabs;
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
                var prefabs = assetBundle.LoadAllAssets<GameObject>();
                prefabs.ToList().ForEach(p => {
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
