
using System.Diagnostics;

namespace CForth
{
    public class VirtualMachine
    {
        internal StreamWriter writer;
        internal CForthEnv env;
        internal int exitCode;

        internal void Log(object msg) { if (env.logging) Console.Write(msg); }
        internal void Error(object msg) => Console.Write(msg);

        public VirtualMachine(CForthEnv env)
        {
            this.env = env;
            Log("[Info] Init CForth vm\n");
        }

        public void Compile()
        {
            writer = new StreamWriter(env.output + ".asm");
            Log("[Info] Compile CForth code to assembly\n");
            InnerCompile();
            Log($"[Info] CForth program compiled successfuly\n");
            Log("[Info] Compile and link assembly code\n");
            Close();

            Process process;

            process = Process.Start(new ProcessStartInfo()
            {
                FileName = "fasm",
                Arguments = $"{env.output}.asm {env.output}",
                RedirectStandardError = !env.assembly,
                RedirectStandardOutput = !env.assembly
            });
            process.WaitForExit();
            exitCode = process.ExitCode;

            if (exitCode == 0)
            {
                process = Process.Start("chmod", $"+x {env.output}");
                process.WaitForExit();
                exitCode = process.ExitCode;
                if (env.run)
                    Run();
            }
            else
            {
                Error($"[Error] Failed to compile assembly code \n");
            }
        }

        internal virtual void InnerCompile() { }

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
                Error($"[Error] No build to run! please compile program first\n");
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
                Error($"[Error] No build to run! please compile program first\n");
            }
        }

        public void Close()
        {
            writer.Close();
        }
    }
}