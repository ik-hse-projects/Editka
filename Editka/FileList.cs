using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Editka.Files;

namespace Editka
{
    public class FileList
    {
        [XmlIgnore] public readonly TreeView TreeView;
        [XmlIgnore] private readonly MainForm _root;

        // For xml serialization. Do not forget to set _root after.
        private FileList()
        {
            TreeView = new TreeView {Dock = DockStyle.Fill};
            TreeView.NodeMouseDoubleClick += OnTreeNodeMouseClickEventHandler;
            TreeView.NodeMouseClick += OnNodeMouseClickEventHandler;
        }

        public FileList(MainForm root) : this()
        {
            _root = root;
        }

        [XmlIgnore] public IEnumerable<BaseNode> TopNodes => TreeView.Nodes.OfType<BaseNode>();

        private void OnTreeNodeMouseClickEventHandler(object sender, TreeNodeMouseClickEventArgs args)
        {
            if (args.Node is OpenedFile openedFile)
            {
                _root.OpenedTabs.Open(openedFile);
            }
        }

        private void OnNodeMouseClickEventHandler(object sender, TreeNodeMouseClickEventArgs args)
        {
            if (args.Button == MouseButtons.Right)
            {
                if (args.Node is OpenedFile openedFile)
                {
                    openedFile.Opened?.Close();
                    openedFile.Dispose();
                }

                args.Node.Remove();
            }
        }

        public IEnumerable<TreeNode> WalkTree()
        {
            return WalkTree(TreeView.Nodes);
        }

        private IEnumerable<TreeNode> WalkTree(TreeNodeCollection parent)
        {
            foreach (TreeNode node in parent)
            {
                yield return node;
                foreach (var children in WalkTree(node.Nodes))
                {
                    yield return children;
                }
            }
        }
    }
}