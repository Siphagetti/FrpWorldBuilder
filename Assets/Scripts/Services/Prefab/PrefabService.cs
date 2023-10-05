using SFB;
using System.IO;
using UnityEngine;

namespace Prefab
{
    internal class PrefabService : IPrefabService
    {
        readonly string destFolder = Path.Combine(Application.dataPath, "Resources");

        // Copies the .obj file into Resources folder.
        public void ImportPrefab()
        {
            string[] sourceDirs = StandaloneFileBrowser.OpenFolderPanel("Choose Asset Folder", "", false);
            if (sourceDirs == null || sourceDirs.Length == 0) return;

            CopyFolder(sourceDirs[0], destFolder);
        }

        // ------------ Helpers ------------
        private void CopyFolder(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            string dirName = Path.GetFileName(sourcePath);
            string destFolder = Path.Combine(destinationPath, dirName);

            if (Directory.Exists(destFolder))
            {
                Debug.LogError("A folder named as " + Path.GetFileName(destFolder) + " already exists");
                return;
            }

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
    }
}
