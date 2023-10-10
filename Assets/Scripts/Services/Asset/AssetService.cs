using SFB;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Build.Content;
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
        private readonly List<string> supportedFileTypes = new()
        {
            ".obj",
            ".fbx"
        };

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

            LoadPrefabs(folderPath, ref list);
            foreach (var subfolderPath in Directory.GetDirectories(folderPath)) LoadPrefabs(subfolderPath, ref list);

            return list;
        }

        public bool CreateCategoryFolder(string relativeFolderPath)
        {
            var destFolder = Path.Combine(_folderPath, relativeFolderPath);

            if (Directory.Exists(destFolder)) { Log.Logger.Log_Fatal("folder_exists", destFolder); return false; }

            Directory.CreateDirectory(destFolder);
            assetCategories.categoryFolderRelativePaths.Add(relativeFolderPath);

            Save.SaveManager.Instance.Save();
            return true;
        }

        public string ImportFolder(string relativeFolderPath)
        {
            // Let user choose import folder then get its path
            string[] sourceDirs = StandaloneFileBrowser.OpenFolderPanel("Choose Asset Folder", "", false);
            if (sourceDirs == null || sourceDirs.Length == 0) return null;

            // Set the destination path
            string destFolderPath = Path.Combine(_folderPath, relativeFolderPath);

            // Copy the selected folder to the destination asynchronously
            bool isCopied = CopyFolder(sourceDirs[0], destFolderPath);
            if (!isCopied) return null;

            string newImportedFolder = Path.Combine(destFolderPath, Path.GetFileName(sourceDirs[0]));

            return newImportedFolder;
        }

        public bool CategoryFolderExists(string category)
        {
            var destFolder = Path.Combine(_folderPath, category);
            return Directory.Exists(destFolder);
        }

        // ------------ Helpers ------------

        private void LoadPrefabs(string folderPath, ref List<GameObject> list)
        {
            foreach (var fileType in supportedFileTypes)
            {
                var files = Directory.GetFiles(folderPath, "*" + fileType).Select(f => f.Replace(fileType, ""));
                foreach (string file in files)
                {
                    GameObject prefab = Resources.Load<GameObject>(GetRelativePath(file));
                    list.Add(prefab);
                }
            }
        }

        private bool CopyFolder(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);

            string dirName = Path.GetFileName(sourcePath);
            string destFolder = Path.Combine(destinationPath, dirName);

            if (Directory.Exists(destFolder)) { Log.Logger.Log_Error("folder_exists", Path.GetFileName(destFolder)); return false; }

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

            return true;
        }

        // Return relative path for Resources folder.
        private string GetRelativePath(string path) => Path.Combine(_subfolderName, path.Replace(_folderPath + Path.DirectorySeparatorChar, ""));
        public List<string> GetAllCategories() => new(assetCategories.categoryFolderRelativePaths);
        public void RemoveCategoryFolder(string category) => assetCategories.categoryFolderRelativePaths.Remove(category);
    }
}
