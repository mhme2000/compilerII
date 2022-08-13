namespace CompilerApp;
public class Symbol
{
    public string Value { get; set; } = string.Empty;
    public EnumTypeToken Type {get; set; }
    public int EndRel {get; init
        ; }
}