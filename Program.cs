using System.Text.RegularExpressions;
using CForth;

public class Program
{
    public static void Main(string[] args)
    {
        CForthEnv env = resolve_args(args);
        if (env.stack)
        {
            CForthIR vm = new CForthIR(env);
            vm.Compile();
        }
        else
        {
            VirtualMachine vm = new VirtualMachine(env);
            vm.Compile();
        }
    }

    private static CForthEnv resolve_args(string[] args)
    {
        CForthEnv env = new CForthEnv();
        if (args.Length == 0)
        {
            Console.WriteLine("[Error] Expected at least one arguments");
            PrintUsege();
            Environment.Exit(1);
        }

        if (args.Contains("-h"))
        {
            PrintUsege();
            Environment.Exit(0);
        }

        Console.WriteLine(args[0]);

        if (!File.Exists(args[0]))
        {
            Console.WriteLine("[Error] Expected input file path at first argument");
            Environment.Exit(1);
        }
        else
        {
            env.main = args[0];
        }

        env.assembly = args.Contains("-al");
        env.logging = args.Contains("-d");
        env.run = args.Contains("-r");
        env.stack = args.Contains("-s");
        env.output = args.Contains("-o") ? args[Array.IndexOf(args, "-o") + 1] : "out";

        return env;
    }

    private static void PrintUsege()
    {
        Console.WriteLine("Usage: CForth <file> [options]");
        Console.WriteLine("<file>");
        Console.WriteLine("     : CForth file path with extention .CForth at the end");
        Console.WriteLine("options");
        Console.WriteLine(" -o  : final output name default is 'out'");
        Console.WriteLine(" -s  : use stack based compiler");
        Console.WriteLine(" -r  : run program right after compiling");
        Console.WriteLine(" -d  : enable logging");
        Console.WriteLine(" -al : enable fasm assembler logging");
        Console.WriteLine(" -h  : print usage");
    }
}