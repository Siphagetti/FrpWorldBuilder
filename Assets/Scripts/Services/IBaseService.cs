using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IBaseService
    {
        virtual public Task Awake() { return Task.CompletedTask; }
        virtual public Task Start() { return Task.CompletedTask; }
    }
}
