using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    internal interface IWorldService : IBaseService
    {
        public Response CreateNewWorld(string worldName);
        public Response ChangeWorld(string worldName);
    }
}
