namespace Editka.Files
{
    public class CSharp : Plain
    {
        public CSharp(string path) : base(path)
        {
        }

        public override FileKind Kind => FileKind.CSharp;

        public Solution? Solution { get; set; }

        protected override string SuggestedExtension()
        {
            return ".cs";
        }

        public void Build(MainForm root)
        {
            var path = GetPath(true);
            if (path == null)
            {
                return;
            }

            root.Notes.BuildLog.DoBuild(path, "csc");
        }
    }
}