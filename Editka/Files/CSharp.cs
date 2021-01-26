namespace Editka.Files
{
    public class CSharp: Plain
    {
        public CSharp(string path): base(path)
        {
        }

        protected override string SuggestedExtension() => ".cs";
    }
}