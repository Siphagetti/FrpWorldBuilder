using SFB;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Prefab
{
    internal class PrefabService : IPrefabService
    {
        readonly string destFolder = Path.Combine(Application.dataPath, "Resources");

        // Copies the .obj file into Resources folder.
        public void ImportPrefab()
        {
            string[] importDirs = StandaloneFileBrowser.OpenFolderPanel("Choose Asset Folder", "", false);
            if (importDirs == null || importDirs.Length == 0) return;

            string importDir = importDirs[0];

            if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
            
            string importDirName = Path.GetFileName(importDir);
            string destPath = Path.Combine(destFolder, importDirName);

            Directory.CreateDirectory(destPath);

            // Copy Files
            foreach (string filePath in Directory.GetFiles(importDir))
            {
                string fileName = Path.GetFileName(filePath);
                string destFilePath = Path.Combine(destPath, fileName);
                File.Copy(filePath, destFilePath);
            }


        }

        public PrefabService()
        {
            // Temporary
            ImportPrefab();
        }

        // ------------ Helpers ------------
        private void CopyFolder(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            string dirName = Path.GetDirectoryName(sourcePath);
            string destFolder = Path.Combine(destinationPath, dirName);

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
