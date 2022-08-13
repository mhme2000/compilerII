namespace CompilerApp;
public static class Program
{
    static void Main(string[] args)
    {
        if (args.Length <= 0) return;
        var mode = args[0];
        var input = args.Length > 1 ? args[1] : "input.txt";
        var output = args.Length > 2 ? args[2] : "output.txt";
        switch (mode)
        {
            case "executor":
                Console.WriteLine("Starting executor...");
                new Executor(output).Execute();
                break;
            case "compiler":
            {
                Console.WriteLine("Starting compiler...");
                new Syntactic(input, output).CheckSyntax();
                break;
            }
        }
    }
}