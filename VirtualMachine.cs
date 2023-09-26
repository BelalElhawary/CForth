
using System.Diagnostics;

namespace CForth
{
    public class VirtualMachine
    {
        StreamWriter writer;
        public CForthEnv env = new CForthEnv();
        int exitCode;
        public void Log(object msg) { if (env.logging) Console.Write(msg); }

        public VirtualMachine(string input, string output)
        {
            Log("[Info] Init CForth vm\n");
            env.output = output;
            env.main = input;
        }

        public void Compile()
        {
            Scanner scanner = new Scanner(File.ReadAllText(env.main));
            var tokens = scanner.scanTokens();
            Parser parser = new Parser(tokens);
            writer = new StreamWriter(env.output + ".asm");
            Compiler compiler = new Compiler(writer);
            compiler.compile(parser.parse());
            Close();

            Process process;

            process = Process.Start(new ProcessStartInfo() { FileName = "fasm", Arguments = $"{env.output}.asm {env.output}", RedirectStandardError = !env.assembly, RedirectStandardOutput = !env.assembly });
            process.WaitForExit();
            exitCode = process.ExitCode;

            if (exitCode == 0)
            {
                process = Process.Start("chmod", $"+x {env.output}");
                process.WaitForExit();
                exitCode = process.ExitCode;
                Run();
            }
            else
            {
                Console.Write($"[Error] Failed to compile assembly code \n");
            }
        }

        public void Run()
        {
            if (env.time)
            {
                RunTime();
            }
            else
            {
                RunNormal();
            }
        }

        public void RunNormal()
        {
            if (File.Exists(env.output))
            {
                Log("[Info] Execute program\n");
                Process process;
                process = Process.Start(env.output);
                process.WaitForExit();
                Log($"[Info] Program executed with return code {process.ExitCode}\n");
            }
            else
            {
                Console.Write($"[Error] No build to run! please compile program first\n");
            }
        }

        public void RunTime()
        {
            if (File.Exists(env.output))
            {
                Log("[Info] Execute program\n");
                Process process;
                process = Process.Start("time", env.output);
                process.WaitForExit();
                Log($"[Info] Program executed with return code {process.ExitCode}\n");
            }
            else
            {
                Console.Write($"[Error] No build to run! please compile program first\n");
            }
        }

        public void Close()
        {
            writer.Close();
        }
    }
}