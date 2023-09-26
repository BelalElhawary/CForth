
using System.Diagnostics;

namespace CForth
{
    public enum INST
    {
        PUSH,
        PLUS,
        MINUS,
        EQUAL,
        DUMP,
        IF,
        ELSE,
        END,
        RETURN,
    }

    public class CForthIR
    {
        StreamWriter writer;
        Stopwatch stopwatch = new Stopwatch();
        List<(INST, object)> program;
        CForthEnv env = new CForthEnv();
        int exitCode;
        public void Log(object msg) { if (env.logging) Console.Write(msg); }

        public CForthIR(CForthEnv env)
        {
            this.env = env;
            Log("[Info] Init CForth vm\n");
        }

        public void Parse()
        {
            program = new List<(INST, object)>();
            var words = File.ReadAllText(env.main).Split(new char[] { ',', '!', '\'', ' ', '\n', '\t' });
            foreach (string word in words)
            {
                Log($"[{word}]");
                switch (word)
                {
                    case "+":
                        program.Add((INST.PLUS, 0));
                        break;
                    case "-":
                        program.Add((INST.MINUS, 0));
                        break;
                    case ".":
                        program.Add((INST.DUMP, 0));
                        break;
                    case "==":
                        program.Add((INST.EQUAL, 0));
                        break;
                    case "if":
                        program.Add((INST.IF, 0));
                        break;
                    case "end":
                        program.Add((INST.END, 0));
                        break;
                    case "else":
                        program.Add((INST.ELSE, 0));
                        break;
                    default:
                        {
                            if (double.TryParse(word, out double result))
                                program.Add((INST.PUSH, result));
                            break;
                        }
                }
            }
            Log('\n');

            crossreferance_program();
        }

        public void crossreferance_program()
        {
            Stack<int> stack = new Stack<int>();
            for (int i = 0; i < program.Count; i++)
            {
                var op = program[i];
                if (op.Item1 == INST.IF)
                {
                    stack.Push(i);
                }
                else if (op.Item1 == INST.ELSE)
                {
                    var if_ip = stack.Pop();
                    program[if_ip] = (INST.IF, i + 1);
                    stack.Push(i);
                }
                else if (op.Item1 == INST.END)
                {
                    var block_ip = stack.Pop();
                    var inst = program[block_ip].Item1;
                    if (inst == INST.IF || inst == INST.ELSE)
                        program[block_ip] = (inst, i);
                    else
                        throw new Exception("end can only close if blocks");
                }
            }
        }

        public void Compile()
        {
            Parse();

            Log("[Info] Compile CForth code to assembly\n");

            writer = new StreamWriter(env.output + ".asm");

            stopwatch.Start();

            writer.Write(Common.HEADER);
            writer.Write(Common.CODE_SEGMENT);
            writer.Write(Common.DUMP_FUNCTION);
            writer.Write(Common.ENTRY);

            for (int i = 0; i < program.Count; i++)
            {
                var inst = program[i];
                switch (inst.Item1)
                {
                    case INST.PUSH:
                        writer.Write(Common.PUSH(inst.Item2));
                        break;
                    case INST.PLUS:
                        writer.Write(Common.PLUS);
                        break;
                    case INST.MINUS:
                        writer.Write(Common.MINUS);
                        break;
                    case INST.DUMP:
                        writer.Write(Common.DUMP);
                        break;
                    case INST.EQUAL:
                        writer.Write(Common.EQUAL);
                        break;
                    case INST.IF:
                        writer.Write(Common.IF(inst.Item2));
                        break;
                    case INST.ELSE:
                        writer.Write(Common.ELSE(inst.Item2, i + 1));
                        break;
                    case INST.END:
                        writer.Write($"addr_{i + 1}:");
                        break;
                }
            }

            writer.Write(Common.PROGRAM_RETURN(0));
            Close();

            stopwatch.Stop();
            Log($"[Info] CForth program compiled in {stopwatch.ElapsedMilliseconds}ms\n");
            Log("[Info] Compile and link assembly code\n");
            // compile to exe

            Process process;

            process = Process.Start(new ProcessStartInfo() { FileName = "fasm", Arguments = $"{env.output}.asm {env.output}", RedirectStandardError = !env.assembly, RedirectStandardOutput = !env.assembly });
            process.WaitForExit();
            exitCode = process.ExitCode;

            if (exitCode == 0)
            {
                process = Process.Start("chmod", $"+x {env.output}");
                process.WaitForExit();
                exitCode = process.ExitCode;
                Log($"[Info] Assembly code compiled\n");
                if (env.run)
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