using System.Linq;
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
                    var existing = root.OpenedTabs.FileTabs.SingleOrDefault(tab => tab.File.Id == openedFile.Id);
                    if (existing == null)
                    {
                        var page = new FileView(root, openedFile);
                        root.OpenedTabs.TabPages.Add(page);
                        existing = page;
                    }

                    root.OpenedTabs.SelectedTab = existing;
                }
            };
            NodeMouseClick += (sender, args) =>
            {
                if (args.Button == MouseButtons.Right && args.Node is OpenedFile openedFile)
                {
                    var existing = root.OpenedTabs.FileTabs.SingleOrDefault(tab => tab.File.Id == openedFile.Id);
                    if (existing != null)
                    {
                        existing.Save();
                        root.OpenedTabs.TabPages.Remove(existing);
                    }

                    args.Node.Remove();
                    openedFile.Dispose();
                }
            };
        }
    }
}