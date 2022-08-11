using System.Collections;
using System.Globalization;

namespace CompilerApp;
public sealed class Syntactic
{
    #region private_variables
    private static Lex _lexScanner = null!;

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

    #region private_methods
    private static double ResolveExpression(string operating1, string operator1, string? operating2 = null)
    {
        return operator1 switch
        {
            "+" => Convert.ToDouble(operating1) + Convert.ToDouble(operating2),
            "-" => Convert.ToDouble(operating1) - Convert.ToDouble(operating2),
            "*" => Convert.ToDouble(operating1) * Convert.ToDouble(operating2),
            "/" => Convert.ToDouble(operating1) / Convert.ToDouble(operating2),
            "^" => Math.Pow(Convert.ToDouble(operating1), Convert.ToDouble(operating2)),
            "exp" => Math.Exp(Convert.ToDouble(operating1)),
            _ => double.NaN
        };
    }
    private static string Calculate(List<ExpressionItem> expressionForCalculate)
    {
        var maxLevel = expressionForCalculate.Max(t => t.Level);
        for (var currentLevel = maxLevel; currentLevel >= 0; currentLevel--)
        {
            if (expressionForCalculate.Count == 1)
            {
                return expressionForCalculate[0].Value;
            }

            var quantityOperatorsInCurrentLevel = expressionForCalculate
                .FindAll(t => t.Level == currentLevel && t.Type == EnumTypeToken.Identifier).Count;
            if (quantityOperatorsInCurrentLevel == 0) continue;
            var operatorsInCurrentLevel = expressionForCalculate
                .FindAll(t => t.Level == currentLevel && t.Type == EnumTypeToken.Identifier);
            foreach (var currentOperator in operatorsInCurrentLevel)
            {
                var positionOperator = expressionForCalculate.FindIndex(t => t.Id == currentOperator.Id);
                if (currentOperator.Value == "exp")
                {
                    var result = ResolveExpression(expressionForCalculate[positionOperator + 1].Value,
                        expressionForCalculate[positionOperator].Value);
                    expressionForCalculate.RemoveAt(positionOperator + 1);
                    expressionForCalculate[positionOperator] = new ExpressionItem()
                    {
                        Id = Guid.NewGuid(),
                        Value = result.ToString(CultureInfo.InvariantCulture),
                        Type = EnumTypeToken.Identifier
                    };
                }
                else
                {
                    var result = ResolveExpression(expressionForCalculate[positionOperator - 1].Value,
                        expressionForCalculate[positionOperator].Value,
                        expressionForCalculate[positionOperator + 1].Value);
                    expressionForCalculate.RemoveRange(positionOperator - 1, 2);
                    expressionForCalculate[positionOperator - 1] = new ExpressionItem()
                    {
                        Id = Guid.NewGuid(),
                        Value = result.ToString(CultureInfo.InvariantCulture),
                        Type = EnumTypeToken.Identifier
                    };
                }

            }
        }
        return string.Empty;
    }

    // private static string? NextExpectedToken(EnumTypeToken currentSymbolType)
    // {
    //     switch (currentSymbolType)
    //     {
    //         case EnumTypeToken.Identifier:
    //             return "'operator' or ')' or ']' or 'LineBreak' or 'EndChain'";
    //         // case EnumTypeToken:
    //         //     return "'Identifier' or '(' or '[' or 'exp'";
    //         // case EnumTypeToken.Bundler:
    //         //     return "'Identifier' or 'operator'";
    //         // case EnumTypeToken.LineBreak:
    //         //     return "'Identifier' or '(' or 'exp'";
    //         // case EnumTypeToken.EndOfChain:
    //         //     return null;
    //         default:
    //             throw new ArgumentOutOfRangeException(nameof(currentSymbolType), currentSymbolType, null);
    //     }
    // }
    #endregion

    public Syntactic(string inputFileName)
    {
        _lexScanner = new Lex(inputFileName);
    }

    public void CheckSyntax()
    {
        var expression = new List<ExpressionItem>();
        var tokenPosition = -1;
        var stack = new Stack<StackItem>();
        var token = _lexScanner.NextToken();
        // var nextExpectedToken = NextExpectedToken(token.Type);
        var lineOfCode = 1;
        stack.Push(new StackItem()
        {
            Content = "$",
            Level = 0,
        });
        stack.Push(new StackItem()
        {
            Content = "programa",
            Level = 0,
        });
        while (stack.Peek().Content != "$")
        {
            var symbolInExpression = token.Content;
            switch (token.Type)
            {
                case EnumTypeToken.Identifier:
                    symbolInExpression = "ident";
                    break;
                case EnumTypeToken.Integer:
                    symbolInExpression = "numero_int";
                    break;
                case EnumTypeToken.Real:
                    symbolInExpression = "numero_real";
                    break;
            }

            // if (token.Type == EnumTypeToken.Identifier)
            // {
            //     if (expression.Count > 0)
            //     {
            //         Console.WriteLine(Calculate(expression));
            //         expression.Clear();
            //         stack.Push(new StackItem()
            //         {
            //             Content = "E",
            //             Level = 0,
            //         });
            //         tokenPosition = -1;
            //         lineOfCode++;
            //     }
            //     token = _lexScanner.NextToken();
            // }
            if (stack.Peek().Content == symbolInExpression)
            {
                // if (token.Type == EnumTypeToken.Identifier || token.Type == EnumTypeToken.Identifier)
                // {
                //     expression.Add(new ExpressionItem()
                //     {
                //         Id = Guid.NewGuid(),
                //         Value = token.Content,
                //         Level = stack.Peek().Level,
                //         Type = token.Type
                //     });
                //     tokenPosition++;
                // }

                stack.Pop();
                token = _lexScanner.NextToken();
            }
            else if (Terminals.Contains(stack.Peek().Content))
            {
                throw new Exception($"Syntactic Error in line {lineOfCode}, '{stack.Peek().Content}' was expected, but found '{token.Content}'");
            }
            else
            {
                var rules = HashtableSyntactic[new KeyHashtable(stack.Peek().Content, symbolInExpression)];
                if (rules == null) throw new Exception($"Syntactic Error in line {lineOfCode},  was expected, but found '{token.Content}'");
                var fatherLevel = stack.Peek().Level;
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
                            Level = fatherLevel + 1,
                        });
                    }
                    // else
                    // {
                    //     if (token.Content != "$" || stack.Peek().Content != "$")
                    //     {
                    //         expression[tokenPosition].Level -= 1;
                    //     }
                    // }
                }
            }
        }

        if (_lexScanner.Position < _lexScanner.ContentFile.Length)
        {
            throw new Exception($"Syntactic Error in line {lineOfCode}, was expected, but found '{token.Content}'");
        }

        // Console.WriteLine(Calculate(expression));
        Console.WriteLine("Build success!");
    }
}
