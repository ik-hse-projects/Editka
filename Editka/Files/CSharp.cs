namespace Editka.Files
{
    /// <summary>
    /// .cs файл.
    /// </summary>
    public class CSharp : Plain
    {
        public CSharp(string path) : base(path)
        {
        }

        public override FileKind Kind => FileKind.CSharp;

        /// <summary>
        /// Solution или project, в котором открыт этот файл.
        /// </summary>
        public Solution? Solution { get; set; }

        /// <summary>
        /// Компилирует код.
        /// </summary>
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