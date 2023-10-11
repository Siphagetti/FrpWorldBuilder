using UnityEngine;

namespace UserInterface.World.Building.Prefab
{

#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(PrefabEntity))]
    public class PrefabEntityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Capture and Save Thumbnail"))
            {
                CaptureAndSaveThumbnail((PrefabEntity)target);
            }
        }

        private void CaptureAndSaveThumbnail(PrefabEntity prefabEntity)
        {
            if (prefabEntity.prefab != null)
            {
                Texture2D thumbnail = AssetPreview.GetAssetPreview(prefabEntity.prefab);
                if (thumbnail != null)
                {
                    // Save the thumbnail as a PNG file
                    byte[] pngData = thumbnail.EncodeToPNG();
                    string assetPath = AssetDatabase.GetAssetPath(prefabEntity);
                    string thumbnailPath = assetPath.Substring(0, assetPath.LastIndexOf('/')) + "/" + prefabEntity.prefab.name + "_Thumbnail.png";

                    System.IO.File.WriteAllBytes(thumbnailPath, pngData);

                    // AssetDatabase.Refresh to make the thumbnail available
                    AssetDatabase.Refresh();

                    // Update the prefabThumbnail field
                    prefabEntity.prefabThumbnail = (Texture2D)AssetDatabase.LoadAssetAtPath(thumbnailPath, typeof(Texture2D));
                    EditorUtility.SetDirty(prefabEntity);
                }
            }
        }
    }
#endif

    [CreateAssetMenu(fileName = "New Prefab Entity", menuName = "Custom/PrefabEntity")]
    public class PrefabEntity : ScriptableObject
    {
        public GameObject prefab;
        public Texture2D prefabThumbnail;
    }
}

