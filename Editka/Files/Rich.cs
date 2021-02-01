namespace Editka.Files
{
    public class Rich : OpenedFile
    {
        public Rich(string path) : base(path)
        {
        }

        public Rich()
        {
        }

        public override FileKind Kind => FileKind.Rtf;

        protected override string SuggestedExtension()
        {
            return ".rtf";
        }
    }
}