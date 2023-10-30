using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using World;
using System.IO;
using System;

#if UNITY_EDITOR

using UnityEditor;
using Unity.VisualScripting;

namespace Prefab
{
    public class PrefabModifier : MonoBehaviour
    {
        [MenuItem("Modify Prefabs/Make Textures Readable")]
        public static void MakeTexturesReadable()
        {
            /*
                If textures of a prefab are unreadable, you can use this function in the editor.
            */

            Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();

            foreach (Texture2D texture in textures)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer != null)
                {
                    importer.isReadable = true;
                    AssetDatabase.ImportAsset(path);
                }
            }
        }

        [MenuItem("Modify Prefabs/Make Meshes Readable")]
        public static void MakeMeshesReadable()
        {
            /*
                If the model of a prefab is unreadable, you can use this function in the editor.
            */

            MeshFilter[] meshFilters = Resources.FindObjectsOfTypeAll<MeshFilter>();

            foreach (MeshFilter meshFilter in meshFilters)
            {
                if (meshFilter.IsDestroyed()) continue;
                Mesh mesh = meshFilter.sharedMesh;

                if (mesh != null)
                {
                    string path = AssetDatabase.GetAssetPath(mesh);
                    ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

                    if (importer != null)
                    {
                        importer.isReadable = true;
                        AssetDatabase.ImportAsset(path);
                    }
                }
            }
        }
    }

#endif

    [Serializable]
    public class PrefabDTO
    {
        public string guid;
        public string prefabName;
        public string assetBundleName;
        public string hierarchyGroupName = "";
        public SerializableTransform transform;

        public PrefabDTO() { }
        public PrefabDTO(PrefabDTO other)
        {
            guid = other.guid;
            prefabName = other.prefabName;
            assetBundleName = other.assetBundleName;
            hierarchyGroupName = other.hierarchyGroupName;
            transform = other.transform;
        }
        public override bool Equals(object obj)
        {
            if (obj is PrefabDTO other)
            {
                // Compare all fields except the transform
                return
                    guid == other.guid &&
                    prefabName == other.prefabName &&
                    assetBundleName == other.assetBundleName &&
                    hierarchyGroupName == other.hierarchyGroupName;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 17;
                hashCode = (hashCode * 23) + guid.GetHashCode();
                hashCode = (hashCode * 23) + prefabName.GetHashCode();
                hashCode = (hashCode * 23) + assetBundleName.GetHashCode();
                hashCode = (hashCode * 23) + hierarchyGroupName.GetHashCode();
                return hashCode;
            }
        }
    }

    public class Prefab : MonoBehaviour
    {
        /*
            If a prefab is loaded recently, it must be initialized to standardize.

            If the prefab needs to be instantiated, a wrapper should keep the prefab. 
            So, CreateWrapper functions should be used to instantiate prefab.
        */

        public static float Size { get; } = 5.0f;

        public PrefabDTO Data { get; set; }

        [SerializeField] private float _scaleFactor;
        [SerializeField] private Mesh _combinedMesh;
        [SerializeField] private Material[] _rimMaterials;

        public void UpdateTransform() => Data.transform = transform;

        public bool Initialize(PrefabDTO data, string assetBundlePath)
        {
            if (!CombineMeshes(assetBundlePath)) return false;

            ResizePrefab();
            CreateRimMaterials();

            Data = data;

            return true;

            bool CombineMeshes(string assetBundlePath)
            {
                // Get all mesh filters in the prefab.
                List<MeshFilter> meshFilters = GetComponentsInChildren<MeshFilter>().ToList();
                var rootMeshFilter = GetComponent<MeshFilter>();
                if (rootMeshFilter != null) meshFilters.Add(rootMeshFilter);

                // Create CombineInstance array to keep combine data
                CombineInstance[] combine = new CombineInstance[meshFilters.Count];

                // Get all combine data in the mesh filters.
                for (int i = 0; i < meshFilters.Count; i++)
                {
                    MeshFilter meshFilter = meshFilters[i];

                    // Mesh must be readable 
                    if (meshFilter.sharedMesh.isReadable)
                    {
                        combine[i].mesh = meshFilter.sharedMesh;
                        combine[i].transform = meshFilter.transform.localToWorldMatrix;
                    }
                    else
                    {
                        Log.Logger.Log_Fatal("bundle_has_unreadable_mesh", name, Path.GetFileName(assetBundlePath));
                        Log.Logger.Log_Error("mesh_unreadable", meshFilter.name, gameObject.name);
                        return false;
                    }
                }

                _combinedMesh = new Mesh();
                _combinedMesh.CombineMeshes(combine, false, true, false);
                return true;
            }

            void ResizePrefab()
            {
                // Get bounds of the combined mesh
                Bounds bounds = _combinedMesh.bounds;

                // Find the largest dimension (x, y, or z)
                float largestDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

                // Calculate the scaling factor to match the largest dimension with the cube's edge length
                _scaleFactor = Size / largestDimension;

                // Apply the scaling factor to the prefab
                transform.localScale = _scaleFactor * Vector3.one;
            }

            void CreateRimMaterials()
            {
                List<MeshRenderer> meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
                var rootRenderer = GetComponent<MeshRenderer>();
                if (rootRenderer != null) meshRenderers.Add(rootRenderer);

                _rimMaterials = new Material[meshRenderers.Count];
                Shader shader = FindObjectOfType<PrefabManager>().rimShader;

                for (int i = 0; i < meshRenderers.Count; i++)
                {
                    _rimMaterials[i] = new Material(shader);
                }
            }
        }

        public GameObject CreateWrapper(Vector3 createPos)
        {
            // Instantiate prefab
            GameObject spawnedPrefab = Instantiate(gameObject);

            // Create wrapper
            GameObject wrapper = new GameObject("Wrapper_" + name);
            InitializeWrapper(wrapper, spawnedPrefab, createPos);

            // Add mesh components to the wrapper
            wrapper.AddComponent<MeshFilter>().mesh = _combinedMesh;
            var meshRenderer = wrapper.AddComponent<MeshRenderer>();
            meshRenderer.materials = _rimMaterials;
            meshRenderer.enabled = false;

            // Transfer the Prefab class properties to the wrapper
            Prefab prefabComponent = wrapper.AddComponent<Prefab>();
            prefabComponent.Data = new(Data);

            // Remove the Prefab component from the spawnedPrefab
            Destroy(spawnedPrefab.GetComponent<Prefab>());

            CreateBoxCollider(wrapper);

            return wrapper;

            void InitializeWrapper(GameObject wrapper, GameObject spawnedPrefab, Vector3 createPos)
            {
                wrapper.layer = LayerMask.NameToLayer("Draggable");
                wrapper.transform.position = createPos;
                wrapper.transform.localScale = _scaleFactor * Vector3.one;

                // Make prefab a child of the wrapper
                wrapper.transform.SetParent(spawnedPrefab.transform.parent);
                spawnedPrefab.transform.SetParent(wrapper.transform);
                spawnedPrefab.transform.localPosition = Vector3.zero;
            }

            void CreateBoxCollider(GameObject wrapper)
            {
                // Get bounds of the combined mesh
                Bounds bounds = _combinedMesh.bounds;

                // Create Box Collider
                BoxCollider boxCollider = wrapper.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(0.8f * bounds.size.x, bounds.size.y, 0.8f * bounds.size.z);
                boxCollider.center = bounds.center;
                boxCollider.isTrigger = true;
            }
        }
    }
}
