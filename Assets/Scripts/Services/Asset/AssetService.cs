using SFB;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Asset
{
    internal class AssetService : IAseetService
    {
        public static readonly string _folderPath = Path.Combine(Path.Combine(Application.dataPath, "Resources"), "Assets");

        public List<GameObject> GetPrefabsInFolder(string folderPath)
        {
            /*
                Loads files that can be GameObject first.
                Then, look subfolders for the files that can be GameObject.
            */

            List<GameObject> list = new();

            LoadPrefabs(folderPath);

            foreach (var subfolder in Directory.GetDirectories(folderPath)) LoadPrefabs(subfolder);

            return list;

            void LoadPrefabs(string folderPath)
            {
                string[] objFiles = Directory.GetFiles(folderPath, "*.obj").Select(f => f.Replace(".obj", "")).ToArray();
                string[] fbxFiles = Directory.GetFiles(folderPath, "*.fbx").Select(f => f.Replace(".fbx", "")).ToArray();

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

        public void CreateFolder(string destPath)
        {
            if (Directory.Exists(destPath)) { Log.Logger.Log_Fatal("folder_exists", Path.GetFileName(destPath)); return; }

            Directory.CreateDirectory(destPath);
            Log.Logger.Log_Info("folder_created", Path.GetFileName(destPath));
        }

        public void ImportFolder(string destPath)
        {
            string[] sourceDirs = StandaloneFileBrowser.OpenFolderPanel("Choose Asset Folder", "", false);
            if (sourceDirs == null || sourceDirs.Length == 0) return;

            CopyFolder(sourceDirs[0], destPath);
            Log.Logger.Log_Info("folder_imported");
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
        private string GetRelativePath(string path) => Path.Combine("Assets", path.Replace(_folderPath + Path.DirectorySeparatorChar, ""));
    }
}
