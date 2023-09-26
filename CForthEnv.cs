using System.IO;
using System.Text.Json;
namespace CForth
{
    public struct CForthEnv
    {
        public bool time;
        public bool logging;
        public bool assembly;
        public bool stack;
        public bool run;
        public string main;
        public string output;
    }
}