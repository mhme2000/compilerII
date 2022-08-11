namespace CompilerApp;
public static class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting compiler...");
        Console.WriteLine("Começou");
        var tokenizer = new Lex("input.txt");
        Token token = null;
        do
        {
            token = tokenizer.NextToken();
            if (token != null)
            {
                Console.WriteLine(token.Content + "-" + token.Type);
            }
        } while (token != null && token.Content != "$");
        var fileName = args.Length > 0 ? args[0] : @"input.txt";
        var syntactic = new Syntactic(fileName);
        syntactic.CheckSyntax();
    }
    // private static void Main(string[] args)
    // {
    //     Console.WriteLine("Começou");
    //     var tokenizer = new Lex("input.txt");
    //     Token token = null;
    //     do
    //     {
    //         token = tokenizer.NextToken();
    //         if (token != null)
    //         {
    //             Console.WriteLine(token.Content + "-" + token.Type);
    //         }
    //     } while (token != null && token.Content != "$");
    // }
}