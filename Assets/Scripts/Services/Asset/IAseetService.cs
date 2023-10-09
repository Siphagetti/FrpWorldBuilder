﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Asset
{
    internal interface IAseetService : Services.IBaseService
    {
        public bool CreateCategoryFolder(string destPath);
        public bool ImportFolder(string destPath);
        public List<GameObject> GetPrefabsInFolder(string folderPath);
        public List<string> GetAllCategories();
        public bool CategoryFolderExists(string category);
        public void RemoveCategoryFolder(string category);
    }
}
