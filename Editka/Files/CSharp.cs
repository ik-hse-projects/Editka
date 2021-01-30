namespace Editka.Files
{
    public class CSharp: Plain
    {
        public CSharp(string path): base(path)
        {
        }

        public override FileKind Kind => FileKind.CSharp;
        protected override string SuggestedExtension() => ".cs";
        
        public Solution? Solution { get; set; }
    }
}