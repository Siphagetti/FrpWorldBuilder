//using UnityEngine;
//using System.IO;

//namespace UserInterface.World.Building.Prefab
//{

//#if UNITY_EDITOR
//    using UnityEditor;

//    [CustomEditor(typeof(PrefabEntity))]
//    public class PrefabEntityEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            PrefabEntity prefabEntity = (PrefabEntity)target;

//            // Set category to the name of the parent folder if the scriptable object is saved.
//            string path = AssetDatabase.GetAssetPath(prefabEntity);
//            string category = string.Empty;

//            if (!string.IsNullOrEmpty(path))
//            {
//                category = Path.GetFileName(Path.GetDirectoryName(path));
//            }

//            prefabEntity.category = category;

//            // Display the default inspector.
//            DrawDefaultInspector();

//            // Display a help message if the Texture2D is null.
//            if (prefabEntity.prefabThumbnail == null)
//            {
//                EditorGUILayout.HelpBox("You need to capture a thumbnail by clicking the 'Capture Thumbnail' button after selecting a prefab.", MessageType.Info);
//            }

//            // Add a button to capture the thumbnail.
//            if (GUILayout.Button("Capture Thumbnail"))
//            {
//                prefabEntity.CaptureAndSaveThumbnail();
//            }

//            EditorUtility.SetDirty(this);
//        }
//    }

//#endif

//    [CreateAssetMenu(fileName = "New Prefab Entity", menuName = "Custom/PrefabEntity")]
//    public class PrefabEntity : ScriptableObject
//    {
//        public string category;
//        public GameObject prefab;
//        public Texture2D prefabThumbnail;

//#       if UNITY_EDITOR
//        private void OnValidate()
//        {
           
//            string assetPath = AssetDatabase.GetAssetPath(this);
//            category = Path.GetFileName(Path.GetDirectoryName(assetPath));

//            if (prefab == null) return;

//            Texture2D thumbnail = AssetPreview.GetAssetPreview(prefab);

//            if (thumbnail == null) return;

//            if (prefabThumbnail != null)
//            {
//                var prevThumbnailPath = AssetDatabase.GetAssetPath(prefabThumbnail);
//                File.Delete(prevThumbnailPath);
//            }

//            // Save the thumbnail as a PNG file
//            byte[] pngData = thumbnail.EncodeToPNG();
//            string thumbnailPath = assetPath.Substring(0, assetPath.LastIndexOf('/')) + "/" + prefab.name + "_Thumbnail.png";

//            File.WriteAllBytes(thumbnailPath, pngData);

//            // AssetDatabase.Refresh to make the thumbnail available
//            AssetDatabase.Refresh();

//            // Update the prefabThumbnail field
//            prefabThumbnail = (Texture2D)AssetDatabase.LoadAssetAtPath(thumbnailPath, typeof(Texture2D));
//        }

//        public void CaptureAndSaveThumbnail()
//        {
//            if (prefab != null)
//            {
//                Texture2D thumbnail = AssetPreview.GetAssetPreview(prefab);
//                if (thumbnail != null)
//                {
//                    if (prefabThumbnail != null)
//                    {
//                        var prevThumbnailPath = AssetDatabase.GetAssetPath(prefabThumbnail);
//                        File.Delete(prevThumbnailPath);
//                    }

//                    // Save the thumbnail as a PNG file
//                    byte[] pngData = thumbnail.EncodeToPNG();
//                    string assetPath = AssetDatabase.GetAssetPath(this);
//                    string thumbnailPath = assetPath.Substring(0, assetPath.LastIndexOf('/')) + "/" + prefab.name + "_Thumbnail.png";

//                    File.WriteAllBytes(thumbnailPath, pngData);

//                    // Force a refresh of the asset database
//                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

//                    // Update the prefabThumbnail field
//                    prefabThumbnail = (Texture2D)AssetDatabase.LoadAssetAtPath(thumbnailPath, typeof(Texture2D));
//                }
//            }
//        }
//#endif
//    }
//}

