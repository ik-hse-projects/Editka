using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Editka.Files
{
    public class OpenedDirectory : BaseNode
    {
        private readonly Dictionary<string, TreeNode> directories = new Dictionary<string, TreeNode>();

        public OpenedDirectory(string root)
        {
            Path = root;

            directories.Add(root, this);
            foreach (var path in Directory
                .EnumerateFiles(root, "*.*", SearchOption.AllDirectories)
                .Where(f => IsKnownExtension(System.IO.Path.GetExtension(f))))
            {
                var dir = GetDirFor(path);
                var opened = Open(path, true);
                if (opened != null)
                {
                    dir.Nodes.Add(opened);
                }
            }
        }

        private TreeNode GetDirFor(string path)
        {
            var parent = System.IO.Path.GetDirectoryName(path);

            if (directories.TryGetValue(parent, out var result))
            {
                return result;
            }

            var name = System.IO.Path.GetFileName(parent);
            var node = new TreeNode(name);

            var dir = GetDirFor(parent);
            dir.Nodes.Add(node);

            directories[path] = dir;

            return node;
        }
    }
}