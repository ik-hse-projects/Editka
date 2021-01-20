using System.Windows.Forms;
using Editka.Files;

namespace Editka
{
    public class FileView : TabPage
    {
        private MainForm _root;
        public OpenedFile File;
        public RichTextBox TextBox;

        public FileView(MainForm root, OpenedFile openedFile)
        {
            File = openedFile;
            Text = openedFile.Filename;
            _root = root;
            TextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                AcceptsTab = true,
                ContextMenu = MenuCreator.ContextMenu(_root),
                MaxLength = 5*1024*1024/2, // 5Mb of UTF-16.
                WordWrap = false,
            };
            Controls.Add(TextBox);
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }
    }
}