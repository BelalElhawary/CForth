using CForth;

public class Program
{
    public static void Main(string[] args)
    {
        VirtualMachine vm = new VirtualMachine($"test/test.CForth", "test/out");
        vm.env.assembly = false;
        vm.Compile();
    }

    private static void resolve_args(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("[Error] Expected two arguments");
            PrintUsege();
            Environment.Exit(1);
        }
    }

    private static void PrintUsege()
    {
        Console.WriteLine("Usage: CForth <file> [options]");
        Console.WriteLine("     : file = CForth file path with extention .CForth at the end");
        Console.WriteLine("options");
        Console.WriteLine(" -o  : final executable name dont provide any extention at the end if it default name is 'out'");
    }
}