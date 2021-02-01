using System.Linq;
using System.Windows.Forms;

namespace Editka.Files
{
    /// <summary>
    /// .sln или .csproj файл. В программе играет роль директории.
    /// </summary>
    public class Solution : OpenedDirectory
    {
        public Solution(string path) : base(System.IO.Path.GetDirectoryName(path)!)
        {
            var nodes = Nodes.OfType<TreeNode>().ToList();
            while (nodes.Count != 0)
            {
                var top = nodes[nodes.Count - 1];
                nodes.RemoveAt(nodes.Count - 1);
                if (top is CSharp cs)
                {
                    cs.Solution = this;
                }

                nodes.AddRange(top.Nodes.OfType<TreeNode>());
            }

            Path = path;
        }

        /// <summary>
        /// Компилирует всё при помощи dotnet. 
        /// </summary>
        public void Build(MainForm root)
        {
            if (Path == null)
            {
                return;
            }

            root.Notes.BuildLog.DoBuild(Path, "dotnet");
        }
    }
}