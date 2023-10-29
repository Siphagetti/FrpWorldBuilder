using System.Collections.Generic;
using System.Linq;

namespace Prefab
{
    public class PrefabRepository
    {
        private Dictionary<string, Dictionary<string, Prefab[]>> _assetBundles;

        public PrefabRepository(IEnumerable<string> categories)
        {
            _assetBundles = categories.ToDictionary(category => category, _ => new Dictionary<string, Prefab[]>());
        }

        public void AddAssetBundle(string category, string bundleName, Prefab[] value)
        {
            if (_assetBundles.ContainsKey(category))
            {
                _assetBundles[category][bundleName] = value;
            }
        }

        public void AddCategory(string category)
        {
            if (!_assetBundles.ContainsKey(category))
            {
                _assetBundles.Add(category, new());
            }
        }

        public void RemoveCategory(string category)
        {
            if (_assetBundles.ContainsKey(category))
            {
                _assetBundles.Remove(category);
            }
        }

        public void RemoveAssetBundle(string bundleName)
        {
            foreach (var assetBundles in _assetBundles.Values)
            {
                if (assetBundles.ContainsKey(bundleName))
                {
                    assetBundles.Remove(bundleName);
                    return;
                }
            }
        }

        public void RemoveAssetBundle(string category, string bundleName)
        {
            if (_assetBundles.ContainsKey(category))
            {
                _assetBundles[category].Remove(bundleName);
            }
        }

        public Prefab GetPrefab(string bundleName, string prefabName)
        {
            foreach (var assetBundles in _assetBundles.Values)
            {
                if (assetBundles.ContainsKey(bundleName))
                {
                    return assetBundles[bundleName].FirstOrDefault(p => p.Data.prefabName == prefabName);
                }
            }

            return null;
        }

        public Prefab GetPrefab(string category, string bundleName, string prefabName)
        {
            if (_assetBundles.ContainsKey(category) && _assetBundles[category].ContainsKey(bundleName))
            {
                return _assetBundles[category][bundleName].FirstOrDefault(p => p.Data.prefabName == prefabName);
            }

            return null;
        }

        public Prefab[] GetPrefabs(string bundleName)
        {
            foreach (var assetBundles in _assetBundles.Values)
            {
                if (assetBundles.ContainsKey(bundleName))
                {
                    return assetBundles[bundleName];
                }
            }

            return null;
        }

        public Prefab[] GetPrefabs(string category, string bundleName)
        {
            if (_assetBundles.ContainsKey(category) && _assetBundles[category].ContainsKey(bundleName))
            {
                return _assetBundles[category][bundleName];
            }

            return null;
        }

        public List<Prefab> GetAllPrefabs(string category)
        {
            if (!_assetBundles.ContainsKey(category))
            {
                return null;
            }

            return _assetBundles[category].Values.SelectMany(prefabArray => prefabArray).ToList();
        }

        public Dictionary<string, Prefab[]> GetAssetBundles(string category)
        {
            if (!_assetBundles.ContainsKey(category))
            {
                return null;
            }

            return _assetBundles[category];
        }
    }
}
