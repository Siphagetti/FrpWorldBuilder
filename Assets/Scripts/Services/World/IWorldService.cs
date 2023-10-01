using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World
{
    internal interface IWorldService : Services.IBaseService
    {
        public Response CreateNewWorld(string worldName);
        public Response ChangeWorld(string worldName);
    }
}
