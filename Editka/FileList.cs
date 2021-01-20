using System.Windows.Forms;
using Editka.Files;

namespace Editka
{
    public class FileList : TreeView
    {
        public FileList(MainForm root)
        {
            NodeMouseDoubleClick += (sender, args) =>
            {
                if (args.Node is OpenedFile openedFile)
                {
                    var page = new FileView(root, openedFile);
                    root.OpenedTabs.TabPages.Add(page);
                    root.OpenedTabs.SelectedTab = page;
                }
            };
        }
    }
}