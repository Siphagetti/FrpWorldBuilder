using SFB;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Asset
{
    // Keeps the created categories' folder paths relative to 'AssetService._folderPath'
    class AssetCategories : Save.SavableObject
    {
        public List<string> categoryFolderRelativePaths = new();

        public AssetCategories() : base(key: "Categories") { }
    }

    internal class AssetService : IAseetService
    {
        private static readonly string _subfolderName = "Assets";
        public static readonly string _folderPath = Path.Combine(Path.Combine(Application.dataPath, "Resources"), _subfolderName);

        private AssetCategories assetCategories = new();

        public List<GameObject> GetPrefabsInFolder(string relativeFolderPath)
        {
            /*
                Loads files that can be GameObject first.
                Then, search subfolders for the files that can be GameObject to load them.

                * relativeFolderPath is relative to '_folderPath', so does not contain '_subfolderName'
            */

            List<GameObject> list = new();

            string folderPath = Path.Combine(_folderPath, relativeFolderPath);

            if(!Directory.Exists(folderPath)) { Log.Logger.Log_Error("category_folder_not_found", relativeFolderPath); return null; }

            LoadPrefabs(folderPath);
            foreach (var subfolderPath in Directory.GetDirectories(folderPath)) LoadPrefabs(subfolderPath);

            return list;

            void LoadPrefabs(string folderPath)
            {
                var objFiles = Directory.GetFiles(folderPath, "*.obj").Select(f => f.Replace(".obj", ""));
                var fbxFiles = Directory.GetFiles(folderPath, "*.fbx").Select(f => f.Replace(".fbx", ""));

                foreach (string file in objFiles)
                {
                    GameObject prefab = Resources.Load<GameObject>(GetRelativePath(file));
                    list.Add(prefab);
                }

                foreach (string file in fbxFiles)
                {
                    GameObject prefab = Resources.Load<GameObject>(GetRelativePath(file));
                    list.Add(prefab);
                }
            }
        }

        public bool CreateCategoryFolder(string relativeFolderPath)
        {
            var destFolder = Path.Combine(_folderPath, relativeFolderPath);

            if (Directory.Exists(destFolder)) { Log.Logger.Log_Fatal("folder_exists", Path.GetFileName(destFolder)); return false; }

            Directory.CreateDirectory(destFolder);
            assetCategories.categoryFolderRelativePaths.Add(relativeFolderPath);

            Save.SaveManager.Instance.Save();
            return true;
        }

        public bool ImportFolder(string relativeFolderPath)
        {
            string[] sourceDirs = StandaloneFileBrowser.OpenFolderPanel("Choose Asset Folder", "", false);
            if (sourceDirs == null || sourceDirs.Length == 0) return false;

            string destPath = Path.Combine(_folderPath, relativeFolderPath);
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
                if (!assetCategories.categoryFolderRelativePaths.Contains(relativeFolderPath))
                     assetCategories.categoryFolderRelativePaths.Add(relativeFolderPath);
            }

            CopyFolder(sourceDirs[0], destPath);
            Log.Logger.Log_Info("folder_imported");
            return true;
        }

        // ------------ Helpers ------------
        private void CopyFolder(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);

            string dirName = Path.GetFileName(sourcePath);
            string destFolder = Path.Combine(destinationPath, dirName);

            if (Directory.Exists(destFolder)) { Log.Logger.Log_Error("folder_exists", Path.GetFileName(destFolder)); return; }

            Directory.CreateDirectory(destFolder);

            // Copy files
            foreach (string filePath in Directory.GetFiles(sourcePath))
            {
                string fileName = Path.GetFileName(filePath);
                string destinationFilePath = Path.Combine(destFolder, fileName);
                File.Copy(filePath, destinationFilePath);
            }

            // Copy subdirectories
            foreach (string subDirectoryPath in Directory.GetDirectories(sourcePath))
            {
                string subDirectoryName = Path.GetFileName(subDirectoryPath);
                string destinationSubDirectoryPath = Path.Combine(destFolder, subDirectoryName);
                CopyFolder(subDirectoryPath, destinationSubDirectoryPath);
            }
        }

        // Return relative path for Resources folder.
        private string GetRelativePath(string path) => Path.Combine(_subfolderName, path.Replace(_folderPath + Path.DirectorySeparatorChar, ""));
        public List<string> GetAllCategories() => new(assetCategories.categoryFolderRelativePaths);
    }
}
