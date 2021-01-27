namespace Editka.Files
{
    public class OpenedDirectory : BaseNode
    {
        public override bool IsOpened => false;

        public OpenedDirectory(string path)
        {
            Path = path;
        }
    }
}