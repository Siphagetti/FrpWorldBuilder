using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log
{
    internal interface ILoggerUI
    {
        public void Log_Track(string text);

        public void Log_Info(string text);

        public void Log_Warning(string text);

        public void Log_Error(string text);

        public void Log_Fatal(string text);
    }
}
