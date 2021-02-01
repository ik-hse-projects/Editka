namespace Editka.Files
{
    public class Plain : OpenedFile
    {
        public Plain(string path) : base(path)
        {
        }

        public Plain()
        {
        }

        public override FileKind Kind => FileKind.Plain;

        protected override string SuggestedExtension()
        {
            return ".txt";
        }
    }
}