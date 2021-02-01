namespace Editka.Files
{
    /// <summary>
    /// .rtf файл.
    /// </summary>
    public class Rich : OpenedFile
    {
        public Rich(string path) : base(path)
        {
        }

        public Rich()
        {
        }

        public override FileKind Kind => FileKind.Rtf;
    }
}