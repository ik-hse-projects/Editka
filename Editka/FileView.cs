using System.Drawing;
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
            Text = openedFile.Filename.Value;
            openedFile.Filename.Changed += (oldValue, newValue) => Text = newValue;
            _root = root;
            TextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                AcceptsTab = true,
                ContextMenu = MenuCreator.ContextMenu(_root),
                MaxLength = 5 * 1024 * 1024 / 2, // 5Mb of UTF-16.
                WordWrap = false,
                Font = new Font(FontFamily.GenericMonospace, 12)
            };
            File.FillTextbox(TextBox);
            Controls.Add(TextBox);
        }

        public void Save()
        {
            File.LoadTextbox(TextBox);
        }
    }
}