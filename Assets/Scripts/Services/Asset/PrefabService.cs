using Dummiesman;
using System.IO;
using UnityEngine;

namespace Prefab
{
    internal class PrefabService : IPrefabService
    {
        /*
         *      Creates a prefab by given file as "sourcePath"
         *      Creates subdirectory by given "relatedFolderPath"
         *      
         *      Destination adjusted in SaveManager
         */
        public void SavePrefab(string sourcePath, string relatedFolderPath = "")
        {
            PrefabLoader.SavePrefab(sourcePath, relatedFolderPath);
        }

        public GameObject LoadPrefab(string name, string relatedFolderPath = "")
        {
            return PrefabLoader.LoadPrefab(name, relatedFolderPath);
        }



        /*
         
               Pathler test için

         */

        private class PrefabLoader : MonoBehaviour
        {
            private readonly static string _folderPath = Path.Combine(Application.dataPath, "ImportedPrefabs");

            public static void SavePrefab(string sourcePath, string relatedPath)
            {
                if (!File.Exists(sourcePath))
                {
                    Debug.LogError("File not found: " + sourcePath);
                    return;
                }

                // -------- Create Folder --------
                string destPath = Path.Combine(_folderPath, relatedPath);

                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                    Debug.Log($"Folder '{destPath}' has been created.");
                }

                // -------- File --------
                string filePath = Path.Combine(destPath, Path.GetFileName(sourcePath));
                File.Copy(sourcePath, filePath);
            }

            public static GameObject LoadPrefab(string name, string relatedPath)
            {
                string sourcePath = Path.Combine(Path.Combine(_folderPath, relatedPath), name + ".obj");

                if (!File.Exists(sourcePath))
                {
                    Debug.LogError("File not found: " + sourcePath);
                    return null;
                }

                GameObject objModel = new OBJLoader().Load(sourcePath);
                return objModel;
            }
        }
    }
}
