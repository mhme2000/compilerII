namespace CompilerApp;
public class Lex
{
    #region variables
    public readonly char[] ContentFile;
    private string _buffer = string.Empty;
    private int _state;
    public int Position;
    #endregion
    
    #region private_methods
    private static bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    private static bool IsLetter(char c)
    {
        return c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');
    }

    private static bool IsSpace(char c)
    {
        return c is ' ' or '\t' or '\r' or '\n';
    }

    private static bool IsSymbol(char c)
    {
        var symbols = new List<char>
        {
            '(', ')', '.', ',', ':', ';', '=', '-', '+', '*', '/'
        };
        return (symbols.Contains(c));
    }

    private static bool IsReservedKeyword(string c)
    {
        var reservedKeywords = new List<string>
        {
            "read", "write", "program", "begin", "end", "real", "integer"
        };
        return reservedKeywords.Contains(c);
    }
    
    private static bool IsDot(char c)
    {
        return c == '.';
    }

    private bool IsEof()
    {
        return Position >= ContentFile.Length;
    }

    private char GoNext()
    {
        if (IsEof())
        {
            return (char)0;
        }
        return ContentFile[Position++];
    }

    private void GoBack()
    {
        Position--;
    }   

    private void ClearBuffer()
    {
        _buffer = string.Empty;
    }
    #endregion
    
    public Lex(string inputFileName)
    {
        try
        {
            using var sr = new StreamReader(inputFileName);
            ContentFile = sr.ReadToEnd().ToCharArray();
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
        ContentFile = File.ReadAllText(inputFileName).ToCharArray();
    }

    public Token NextToken()
    {
        if (IsEof())
        {
            return null!;
        }
        ClearBuffer();
        _state = 0;
        while (true)
        {
            if (IsEof())
            {
                Position = ContentFile.Length + 1;
            }
            var currentChar = GoNext();
            switch (_state)
            {
                case 0:
                    if (IsSpace(currentChar))
                    {
                        _state = 0;
                    }
                    else if (IsLetter(currentChar))
                    {
                        _state = 1;
                        _buffer += currentChar;
                    }
                    else if (IsSymbol(currentChar))
                    {
                        _state = currentChar == ':' ? 6 : 5;
                        _buffer += currentChar;
                    }
                    else if (IsDigit(currentChar))
                    {
                        _state = 2;
                        _buffer += currentChar;
                    }
                    else if (currentChar == '{')
                    {
                        _state = 8;
                        _buffer += currentChar;
                    }
                    else
                    {
                        return null!;
                    }
                    break;
                case 1:
                    if (IsReservedKeyword(_buffer))
                    {
                        GoBack();
                        return new Token(){Type = EnumTypeToken.ReservedKeyword, Content = _buffer};
                    }
                    if (IsLetter(currentChar) || IsDigit(currentChar))
                    {
                        _buffer += currentChar;
                    }
                    else
                    {
                        GoBack();
                        return new Token(){Type = EnumTypeToken.Identifier, Content = _buffer};
                    }
                    break;
                case 2:
                    if (IsDigit(currentChar))
                    {
                        _buffer += currentChar;
                    }
                    else if (IsDot(currentChar))
                    {
                        _state = 3;
                        _buffer += currentChar;
                    }
                    else
                    {
                        GoBack();
                        return new Token(){Type = EnumTypeToken.Integer, Content = _buffer};
                    }
                    break;
                case 3:
                    if (IsDigit(currentChar))
                    {
                        _state = 4;
                        _buffer += currentChar;
                    }
                    else
                    {
                        throw new Exception("Unexpected token");
                    }
                    break;
                case 4:
                    if (IsDigit(currentChar))
                    {
                        _buffer += currentChar;   
                    }
                    else
                    {
                        GoBack();
                        return new Token(){Type = EnumTypeToken.Real, Content = _buffer};
                    }
                    break;
                case 5:
                    if (_buffer.Contains('/') && currentChar == '*')
                    {
                        _state = 8;
                        _buffer += currentChar;
                    }
                    else
                    {
                        GoBack();
                        return new Token(){Type = EnumTypeToken.Symbol, Content = _buffer};   
                    }

                    break;
                case 6:
                    if (currentChar == '=') 
                    {
                        _state = 7;
                        _buffer += currentChar;
                    }
                    else 
                    {
                        GoBack();
                        return new Token(){Type = EnumTypeToken.Symbol, Content = _buffer};
                    }
                    break;
                case 7:
                    GoBack();
                    return new Token(){Type = EnumTypeToken.Symbol, Content = _buffer};
                case 8:
                    if ((_buffer.Contains("/*") && _buffer.Contains("*/")) || (_buffer.Contains('{') && _buffer.Contains('}')))
                    {
                        ClearBuffer();
                        GoBack();
                        _state = 0;
                    }
                    else
                    {
                        _buffer += currentChar;
                    }
                    break;
            }
        }
    }
}