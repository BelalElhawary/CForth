
using System.Diagnostics;

namespace CForth
{
    public class AstCompiler : VirtualMachine
    {
        public AstCompiler(CForthEnv env) : base(env) { }

        internal override void InnerCompile()
        {
            Scanner scanner = new Scanner(File.ReadAllText(env.main));
            var tokens = scanner.scanTokens();
            Parser parser = new Parser(tokens);
            writer = new StreamWriter(env.output + ".asm");
            Compiler compiler = new Compiler(writer);
            compiler.compile(parser.parse());
        }
    }
}