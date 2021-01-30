using System.IO;
using System.Linq;

namespace Editka.Files
{
    public class OpenedDirectory : BaseNode
    {
        public override bool IsOpened => false;

        public OpenedDirectory(string path)
        {
            Path = path;
            foreach (var directory in Directory.EnumerateDirectories(path).Concat(Directory.EnumerateFiles(path)))
            {
                var node = Open(directory, silent: true);
                if (node != null)
                {
                    Nodes.Add(node);
                }
            }
        }
    }
}