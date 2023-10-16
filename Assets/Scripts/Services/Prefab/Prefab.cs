using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


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
        public static Material rimLightMaterial { get; set; }

        public Material[] _originalMaterials;
        public Material[] _rimLightMaterials;

        public void ChangeMaterialsAsRim() => GetComponent<MeshRenderer>().materials = _rimLightMaterials;
        public void ChangeMaterialsAsOrj() => GetComponent<MeshRenderer>().materials = _originalMaterials;

        public async void Initialize()
        {
            gameObject.layer = LayerMask.NameToLayer("Draggable");

            IEnumerable<Task> loadTasks = new List<Task>()
            {
                CreateMeshCollider(),
                ScaleToVolume()
            };

            await Task.WhenAll(loadTasks);
        }

        private async Task CreateMeshCollider()
        {
            // If root has mesh directly creates a mesh collider and sets its mesh as owned mesh
            if (GetComponent<MeshFilter>())
            {
                MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
            }

            // Else get meshes in the children then combine them for mesh collider
            else
            {
                MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
                CombineInstance[] combine = new CombineInstance[meshFilters.Length];

                List<Material> materials = new List<Material>(); // Store materials of the sub-meshes

                for (int i = 0; i < meshFilters.Length; i++)
                {
                    MeshFilter meshFilter = meshFilters[i];

                    if (meshFilter.sharedMesh.isReadable)
                    {
                        combine[i].mesh = meshFilter.sharedMesh;
                        combine[i].transform = meshFilter.transform.localToWorldMatrix;
                        meshFilter.gameObject.SetActive(false); // Disable child objects

                        // Store the material of the sub-mesh
                        Renderer renderer = meshFilter.GetComponent<Renderer>();
                        if (renderer != null && renderer.sharedMaterial != null) materials.Add(renderer.sharedMaterial);
                    }
                    else Debug.LogWarning("Mesh is not readable: " + meshFilter.name);
                }

                Mesh combinedMesh = new Mesh();
                combinedMesh.CombineMeshes(combine, false, true, false);

                // Create a new MeshFilter for the spawnedPrefab and assign the combined mesh
                MeshFilter newMeshFilter = gameObject.AddComponent<MeshFilter>();
                newMeshFilter.sharedMesh = combinedMesh;

                // Create a new MeshRenderer for the spawnedPrefab
                MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

                // Assign the materials to the new MeshRenderer
                meshRenderer.materials = materials.ToArray();

                MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = combinedMesh;
            }

            await CreateRimMaterials();
        }

        private Task ScaleToVolume()
        {
            float size = 5f;

            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

            float totalVolume = 0f;

            // Calculate the total volume of the selected model
            foreach (MeshRenderer renderer in meshRenderers)
            {
                Mesh mesh = renderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
                Vector3 scale = renderer.transform.lossyScale;

                float meshVolume = mesh.bounds.size.x * mesh.bounds.size.y * mesh.bounds.size.z;
                float scaledVolume = meshVolume * scale.x * scale.y * scale.z;

                totalVolume += scaledVolume;
            }

            // Calculate the scaling factor to achieve a volume of 1
            float scaleFactor = size / totalVolume;

            // Apply the scaling factor to the selected model
            transform.localScale *= Mathf.Pow(scaleFactor, 1f / 3f); // Cube root to maintain proportions

            return Task.CompletedTask;
        }

        private Task CreateRimMaterials()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            
            _originalMaterials = meshRenderer.sharedMaterials;

            List<Material> rimMaterials = new();

            foreach (var material in _originalMaterials)
            {
                Material newRimMaterial = new Material(rimLightMaterial);

                if (material.HasProperty("_Color"))
                {
                    Color color = material.GetColor("_Color");
                    newRimMaterial.SetColor("_Color", color);
                }

                if (material.HasProperty("_MainTex"))
                {
                    Texture mainTexture = material.GetTexture("_MainTex");
                    newRimMaterial.SetTexture("_MainTex", mainTexture);
                }

                if (material.HasProperty("_BumpMap"))
                {
                    Texture bumpMap = material.GetTexture("_BumpMap");
                    newRimMaterial.SetTexture("_BumpMap", bumpMap);
                }

                rimMaterials.Add(newRimMaterial);
            }
            _rimLightMaterials = rimMaterials.ToArray();

            return Task.CompletedTask;
        }
    }
}
