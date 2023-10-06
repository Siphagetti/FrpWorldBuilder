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
        public void CreateFolder(string destPath);
        public void ImportFolder(string destPath);
        public List<GameObject> GetPrefabsInFolder(string folderPath);
    }
}
