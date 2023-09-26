using System.IO;
using System.Text.Json;
namespace CForth
{
    public struct CForthEnv
    {
        public bool time;
        public bool logging;
        public bool assembly;
        public string main;
        public string output;

        public static CForthEnv LoadEnv()
        {
            return JsonSerializer.Deserialize<CForthEnv>(File.ReadAllText(".CForth"));
        }
    }
}