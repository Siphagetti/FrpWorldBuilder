using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;


#if UNITY_EDITOR

using UnityEditor;
using Unity.VisualScripting;

class PrefabModifier
{
    [MenuItem("Modify Prefabs/Make Textures Readable")]
    public static void MakeTexturesReadable()
    {
        /*
            If textures of a prefab is unreadable, you can use this function on editor
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
            If model of a prefab is unreadable, you can use this function on editor
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

namespace Prefab
{
    public class Prefab : MonoBehaviour
    {
        public static float Size { get; } = 5.0f;

        [SerializeField] private float _scaleFactor;
        [SerializeField] private Mesh _combinedMesh;
        [SerializeField] private Material[] _rimMaterials;

        public void Initialize()
        {
            // ------------ Combine Mesh ------------

            // Get all mesh filters in the prefab.
            List<MeshFilter> meshFilters = GetComponentsInChildren<MeshFilter>().ToList();
            var rootMeshFilter = GetComponent<MeshFilter>();
            if (rootMeshFilter) meshFilters.Add(rootMeshFilter);
            

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
                else Debug.LogWarning("Mesh is not readable: " + meshFilter.name + " in " + gameObject.name);
            }

            _combinedMesh = new Mesh();
            _combinedMesh.CombineMeshes(combine, false, true, false);

            // Get bounds of the combined mesh
            Bounds bounds = _combinedMesh.bounds;

            // ------------ Resize Prefab ------------

            // Find the largest dimension (x, y, or z)
            float largestDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

            // Calculate the scaling factor to match the largest dimension with the cube's edge length
            _scaleFactor = Size / largestDimension;

            // Apply the scaling factor to the prefab
            transform.localScale = _scaleFactor * Vector3.one;


            // ------------ Create Rim Materials ------------

            List<MeshRenderer> meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
            var rootRenderer = GetComponent<MeshRenderer>();
            if (rootRenderer != null) meshRenderers.Add(rootRenderer);

            _rimMaterials = new Material[meshRenderers.Count];
            Shader shader = Shader.Find("Custom/Ghost Rim");
            for (int i = 0; i < meshRenderers.Count; i++) _rimMaterials[i] = new Material(shader);
        }

        public GameObject CreateWrapper(Vector3 createPos)
        {
            // Instantiate prefab
            GameObject spawnedPrefab = Instantiate(gameObject);

            // Create wrapper
            GameObject wrapper = new GameObject("Wrapper_" + name);
            wrapper.layer = LayerMask.NameToLayer("Draggable");
            wrapper.transform.position = createPos;
            wrapper.transform.localScale = _scaleFactor * Vector3.one;

            // Make prefab child of the wrapper
            wrapper.transform.SetParent(spawnedPrefab.transform.parent);
            spawnedPrefab.transform.SetParent(wrapper.transform);
            spawnedPrefab.transform.localPosition = Vector3.zero;

            // Add mesh components to the wrapper
            wrapper.AddComponent<MeshFilter>().mesh = _combinedMesh;
            wrapper.AddComponent<MeshRenderer>().materials = _rimMaterials;

            // Create box collider

            // Get bounds of the combined mesh
            Bounds bounds = _combinedMesh.bounds;

            // Create Box Collider
            BoxCollider boxCollider = wrapper.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(0.8f * bounds.size.x, bounds.size.y, 0.8f * bounds.size.z);
            boxCollider.center = bounds.center;
            boxCollider.isTrigger = true;

            return wrapper;
        }
    }
}
