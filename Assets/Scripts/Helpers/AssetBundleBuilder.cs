#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace Helpers
{
    public class AssetBundleBuilder : MonoBehaviour
    {
        [MenuItem ("Assets/Build Asset Bundle")]
        [Obsolete]
        static void Build()
        {
            BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows);
        }
    }
}
#endif