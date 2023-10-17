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
    [System.Serializable]
    public struct Materials
    {
        public Transform owner;
        public Material[] originalMaterials;
        public Material[] rimMaterials;

        public void ChangeMaterialsToOrj() => owner.GetComponent<MeshRenderer>().materials = originalMaterials;
        public void ChangeMaterialsToRim() => owner.GetComponent<MeshRenderer>().materials = rimMaterials;
    }

    public class Prefab : MonoBehaviour
    {
        public static float Size { get; } = 5f;
        public static Material rimLightMaterial { get; set; }

        public List<Materials> materials = new();
        public void ChangeMaterialsAsRim() => materials.ForEach(m => m.ChangeMaterialsToRim());
        public void ChangeMaterialsAsOrj() => materials.ForEach(m => m.ChangeMaterialsToOrj());

        public void Initialize()
        {
            gameObject.layer = LayerMask.NameToLayer("Draggable");

            CreateMeshCollider();
            ScaleToVolume();
            CreateRimMaterials(transform);
        }

        private void CreateMeshCollider()
        {
            if (GetComponent<MeshCollider>()) return;

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

                        // Store the material of the sub-mesh
                        MeshRenderer renderer = meshFilter.GetComponent<MeshRenderer>();
                        if (renderer != null && renderer.sharedMaterials != null) materials.AddRange(renderer.sharedMaterials);
                    }
                    else Debug.LogWarning("Mesh is not readable: " + meshFilter.name);
                }

                Mesh combinedMesh = new Mesh();
                combinedMesh.CombineMeshes(combine, true, true, false);

                MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = combinedMesh;
            }
        }

        private void ScaleToVolume()
        {
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
            float scaleFactor = Size / totalVolume;

            // Apply the scaling factor to the selected model
            transform.localScale *= Mathf.Pow(scaleFactor, 1f / 3f); // Cube root to maintain proportions
        }

        private void CreateRimMaterials(Transform parentTransform)
        {
            if (parentTransform == transform)
            {
                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

                if (meshRenderer != null)
                {
                    var orjMaterials = meshRenderer.sharedMaterials;

                    List<Material> rimMaterialList = new();

                    foreach (var material in orjMaterials)
                    {
                        Material newRimMaterial = new(rimLightMaterial);
                        newRimMaterial.name = "Rim_" + material.name;

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

                        rimMaterialList.Add(newRimMaterial);
                    }

                    var rimMaterials = rimMaterialList.ToArray();

                    materials.Add(new()
                    {
                        owner = transform,
                        originalMaterials = orjMaterials,
                        rimMaterials = rimMaterials
                    });
                }
            }

            foreach (Transform child in parentTransform)
            {
                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();

                if (meshRenderer == null) continue;

                var orjMaterials = meshRenderer.sharedMaterials;

                List<Material> rimMaterialList = new();

                foreach (var material in orjMaterials)
                {
                    Material newRimMaterial = new(rimLightMaterial);
                    newRimMaterial.name = "Rim_" + material.name;

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

                    rimMaterialList.Add(newRimMaterial);
                }

                var rimMaterials = rimMaterialList.ToArray();

                materials.Add(new()
                {
                    owner = child,
                    originalMaterials = orjMaterials,
                    rimMaterials = rimMaterials
                });

                if (child.childCount > 0) CreateRimMaterials(child);
                
            }
        }
    }
}
