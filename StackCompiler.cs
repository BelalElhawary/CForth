
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

    public class StackCompiler : VirtualMachine
    {
        List<(INST, object)> program;

        public StackCompiler(CForthEnv env) : base(env) { }

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
                    case "print":
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

        internal override void InnerCompile()
        {

            Parse();

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
        }
    }
}