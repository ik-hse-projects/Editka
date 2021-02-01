using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Editka.Files;

namespace Editka
{
    /// <summary>
    /// Список откртых файлов, но не вкладки.
    /// </summary>
    public class FileList
    {
        /// <summary>
        /// Control, который всё хранит.
        /// </summary>
        public readonly TreeView TreeView;

        private readonly MainForm _root;

        public FileList(MainForm root)
        {
            TreeView = new TreeView {Dock = DockStyle.Fill};
            TreeView.NodeMouseDoubleClick += OpenNode;
            TreeView.NodeMouseClick += MaybeCloseNode;
            _root = root;
        }

        /// <summary>
        /// Файлы/директории, которые открыты сами по себе, а не из-за открытия другой директории.
        /// Т.е. не вложенные.
        /// </summary>
        public IEnumerable<BaseNode> TopNodes => TreeView.Nodes.OfType<BaseNode>();

        private void OpenNode(object sender, TreeNodeMouseClickEventArgs args)
        {
            if (args.Node is OpenedFile openedFile)
            {
                _root.OpenedTabs.Open(openedFile);
            }
        }

        private void MaybeCloseNode(object sender, TreeNodeMouseClickEventArgs args)
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

        /// <summary>
        /// Рекурсивно обходит всё дерево открытых файлов.
        /// </summary>
        public IEnumerable<BaseNode> WalkTree()
        {
            return WalkTree(TreeView.Nodes);
        }

        private IEnumerable<BaseNode> WalkTree(TreeNodeCollection parent)
        {
            foreach (TreeNode node in parent)
            {
                if (node is BaseNode baseNode)
                {
                    yield return baseNode;
                }

                foreach (var children in WalkTree(node.Nodes))
                {
                    yield return children;
                }
            }
        }
    }
}