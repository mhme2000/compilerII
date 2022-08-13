using System.Collections;

namespace CompilerApp;
public sealed class Syntactic
{
    #region private_variables
    private static Lex _lexScanner = null!;
    private List<string> _c = new();
    private int _s = -1;
    private string _outputFileName;
    private readonly Dictionary<string, Symbol> _symbolTable = new();
    private static readonly Hashtable HashtableSyntactic = new()
    {
        { new KeyHashtable("programa", "program"), "program ident corpo ." },
        { new KeyHashtable("corpo", "real"), "dc begin comandos end" },
        { new KeyHashtable("corpo", "integer"), "dc begin comandos end" },
        { new KeyHashtable("dc", "begin"), "&" },
        { new KeyHashtable("dc", "real"), "dc_v mais_dc" },
        { new KeyHashtable("dc", "integer"), "dc_v mais_dc" },
        { new KeyHashtable("dc", "$"), "&" },
        { new KeyHashtable("mais_dc", "begin"), "&" },
        { new KeyHashtable("mais_dc", ";"), "; dc" },
        { new KeyHashtable("mais_dc", "$"), "&" },
        { new KeyHashtable("dc_v", "real"), "tipo_var : variaveis" },
        { new KeyHashtable("dc_v", "integer"), "tipo_var : variaveis" },
        { new KeyHashtable("tipo_var", "real"), "real" },
        { new KeyHashtable("tipo_var", "integer"), "integer" },
        { new KeyHashtable("variaveis", "ident"), "ident mais_var" },
        { new KeyHashtable("mais_var", ";"), "&" },
        { new KeyHashtable("mais_var", "begin"), "&" },
        { new KeyHashtable("mais_var", ","), ", variaveis" },
        { new KeyHashtable("mais_var", "$"), "&" },
        { new KeyHashtable("comandos", "ident"), "comando mais_comandos" },
        { new KeyHashtable("comandos", "read"), "comando mais_comandos" },
        { new KeyHashtable("comandos", "write"), "comando mais_comandos" },
        { new KeyHashtable("mais_comandos", "end"), "&" },
        { new KeyHashtable("mais_comandos", ";"), "; comandos" },
        { new KeyHashtable("mais_comandos", "$"), "&" },
        { new KeyHashtable("comando", "ident"), "ident := expressao" },
        { new KeyHashtable("comando", "read"), "read ( ident )" },
        { new KeyHashtable("comando", "write"), "write ( ident )" },
        { new KeyHashtable("expressao", "end"), "termo outros_termos" },
        { new KeyHashtable("expressao", ";"), "termo outros_termos" },
        { new KeyHashtable("expressao", ")"), "termo outros_termos" },
        { new KeyHashtable("expressao", "-"), "termo outros_termos" },
        { new KeyHashtable("expressao", "ident"), "termo outros_termos" },
        { new KeyHashtable("expressao", "("), "termo outros_termos" },
        { new KeyHashtable("expressao", "numero_int"), "termo outros_termos" },
        { new KeyHashtable("expressao", "numero_real"), "termo outros_termos" },
        { new KeyHashtable("termo", "end"), "op_un fator mais_fatores" },
        { new KeyHashtable("termo", ";"), "op_un fator mais_fatores" }, 
        { new KeyHashtable("termo", ")"), "op_un fator mais_fatores" },
        { new KeyHashtable("termo", "("), "op_un fator mais_fatores" },
        { new KeyHashtable("termo", "-"), "op_un fator mais_fatores" },
        { new KeyHashtable("termo", "+"), "op_un fator mais_fatores" },
        { new KeyHashtable("termo", "ident"), "op_un fator mais_fatores" },
        { new KeyHashtable("termo", "numero_int"), "op_un fator mais_fatores" },
        { new KeyHashtable("termo", "numero_real"), "op_un fator mais_fatores" },
        { new KeyHashtable("op_un", "ident"), "&" },
        { new KeyHashtable("op_un", "("), "&" },
        { new KeyHashtable("op_un", "-"), "-" },
        { new KeyHashtable("op_un", "numero_int"), "&" },
        { new KeyHashtable("op_un", "numero_real"), "&" },
        { new KeyHashtable("op_un", "$"), "&" },
        { new KeyHashtable("fator", "ident"), "ident" },
        { new KeyHashtable("fator", "("), "( expressao )" },
        { new KeyHashtable("fator", "numero_int"), "numero_int" },
        { new KeyHashtable("fator", "numero_real"), "numero_real" },
        { new KeyHashtable("outros_termos", "end"), "&" },
        { new KeyHashtable("outros_termos", ";"), "&" },
        { new KeyHashtable("outros_termos", ")"), "&" },
        { new KeyHashtable("outros_termos", "-"), "op_ad termo outros_termos" },
        { new KeyHashtable("outros_termos", "+"), "op_ad termo outros_termos" },
        { new KeyHashtable("outros_termos", "$"), "&" },
        { new KeyHashtable("op_ad", "-"), "-" },
        { new KeyHashtable("op_ad", "+"), "+" },
        { new KeyHashtable("mais_fatores", "end"), "&" },
        { new KeyHashtable("mais_fatores", ";"), "&" },
        { new KeyHashtable("mais_fatores", ")"), "&" },
        { new KeyHashtable("mais_fatores", "-"), "&" },
        { new KeyHashtable("mais_fatores", "+"), "&" },
        { new KeyHashtable("mais_fatores", "*"), "op_mul fator mais_fatores" },
        { new KeyHashtable("mais_fatores", "/"), "op_mul fator mais_fatores" },
        { new KeyHashtable("mais_fatores", "$"), "&" },
        { new KeyHashtable("op_mul", "*"), "*" },
        { new KeyHashtable("op_mul", "/"), "/" },

    };
    
    private static readonly List<string> Terminals = new()
    {
        "program", "ident", "begin", "end", ".", ";", ":", "real", "integer", "read", "(", ")", "write", ":=", "-", "numero_int", "numero_real", "+", "*", "/", ",", "$"
    };
    #endregion
    
    public Syntactic(string inputFileName, string outputFileName)
    {
        _lexScanner = new Lex(inputFileName);
        _outputFileName = outputFileName;
    }
    
    public void CheckSyntax()
    {
        var stack = new Stack<StackItem>();
        var token = _lexScanner.NextToken();
        var lastIdentifier = string.Empty;
        stack.Push(new StackItem()
        {
            Content = "$",
        });
        stack.Push(new StackItem()
        {
            Content = "programa",
        });
        _c = _c.Append("INPP").ToList();
        var symbolTerminalFather = stack.Peek();
        while (stack.Peek().Content != "$")
        {
            var symbolInExpression = token.Type switch
            {
                EnumTypeToken.Identifier => "ident",
                EnumTypeToken.Integer => "numero_int",
                EnumTypeToken.Real => "numero_real",
                _ => token.Content
            };

            if (stack.Peek().Content == symbolInExpression)
            {
                if (token.Content == ".")
                {
                    _c = _c.Append("PARA").ToList();
                }

                if (symbolTerminalFather.Content == "op_un" && token.Content == "-")
                {
                    _c = _c.Append("INVE").ToList();
                }

                switch (token.Type)
                {
                    case EnumTypeToken.Symbol:
                        _c = token.Content switch
                        {
                            "+" => _c.Append("SOMA").ToList(),
                            "-" => _c.Append("SUBT").ToList(),
                            "*" => _c = _c.Append("MULT").ToList(),
                            "/" =>  _c = _c.Append("DIVI").ToList(),
                            ":=" =>  _c = _c.Append($"ARMZ {_symbolTable[lastIdentifier].EndRel}").ToList(),
                            _ => _c
                        };
                        break;
                    case EnumTypeToken.Identifier:
                        lastIdentifier = token.Content;
                        switch (symbolTerminalFather.Content)
                        {
                            case "variaveis" when _symbolTable.ContainsKey(token.Content):
                                throw new Exception($"Semantic error, identifier '{token.Content}' has already been declared.");
                            case "variaveis":
                                _c = _c.Append("ALME 1").ToList();
                                _symbolTable.Add(token.Content, new Symbol { Type = token.Type, Value = token.Content, EndRel = ++_s});
                                break;
                            case "comando" when !_symbolTable.ContainsKey(token.Content):
                                throw new Exception($"Semantic error, identifier '{token.Content}' was not declared.");
                            case "fator" when !_symbolTable.ContainsKey(token.Content):
                                throw new Exception($"Semantic error, identifier '{token.Content}' was not declared.");
                            case "fator":
                                _c = _c.Append($"CRVL {_symbolTable[lastIdentifier].EndRel}").ToList();
                                break;
                        }

                        break;
                    case EnumTypeToken.ReservedKeyword:
                        _s++; 
                        switch (token.Content)
                        {
                            case "read":
                                _c = _c.Append("LEIT").ToList();
                                _c = _c.Append($"ARMZ {_symbolTable[lastIdentifier].EndRel}").ToList();
                                break;
                            case "write":
                                _c = _c.Append($"CRVL {_symbolTable[lastIdentifier].EndRel}").ToList();
                                _c = _c.Append("IMPR").ToList();
                                break;
                        }

                        break;
                    case EnumTypeToken.Integer or EnumTypeToken.Real:
                        _c = _c.Append($"CRCT {token.Content}").ToList();
                        break;
                }
                stack.Pop();
                token = _lexScanner.NextToken();
            }
            else if (Terminals.Contains(stack.Peek().Content))
            {
                throw new Exception($"Syntactic Error '{stack.Peek().Content}' was expected, but found '{token.Content}'");
            }
            else
            {
                var rules = HashtableSyntactic[new KeyHashtable(stack.Peek().Content, symbolInExpression)];
                if (rules == null) throw new Exception($"Syntactic Error");
                symbolTerminalFather = stack.Peek();
                stack.Pop();
                var rulesSeparated = rules.ToString()?.Split(' ');
                if (rulesSeparated == null) continue;
                for (var i = rulesSeparated.Length - 1; i >= 0; i--)
                {
                    if (rulesSeparated[i] != "&")
                    {
                        stack.Push(new StackItem()
                        {
                            Content = rulesSeparated[i],
                        });
                    }
                }
            }
        }

        if (_lexScanner.Position < _lexScanner.ContentFile.Length)
        {
            throw new Exception($"Syntactic Error");
        }
        File.WriteAllLines(_outputFileName, _c.ToArray());
        Console.WriteLine("Build success!");
    }
}
