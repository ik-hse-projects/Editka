namespace Editka.Files
{
    /// <summary>
    /// .txt файл.
    /// </summary>
    public class Plain : OpenedFile
    {
        public Plain(string path) : base(path)
        {
        }

        public Plain()
        {
        }

        public override FileKind Kind => FileKind.Plain;
    }
}