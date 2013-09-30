namespace GlowBeanGlow.Compiler
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string RawValue { get; set; }
        public int LineNumber { get; set; }
        public int CharacterNumber { get; set; }
    }
}