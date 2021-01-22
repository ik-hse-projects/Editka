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
        public NotifyChanged<bool> Changed = new NotifyChanged<bool>(false);

        public FileView(MainForm root, OpenedFile openedFile)
        {
            File = openedFile;
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

            TextBox.TextChanged += (sender, args) => Changed.Value = true;
            Changed.Changed += (oldValue, newValue) => UpdateText();
            openedFile.Filename.Changed += (oldValue, newValue) => UpdateText();
            UpdateText();
        }

        public void Save()
        {
            File.LoadTextbox(TextBox);
            Changed.Value = false;
        }

        private void UpdateText()
        {
            Text = Changed.Value ? $"{File.Filename.Value}*" : File.Filename.Value;
            var idx = _root.OpenedTabs.TabPages.IndexOf(this);
            if (idx >= 0)
            {
                _root.OpenedTabs.Invalidate(_root.OpenedTabs.GetTabRect(idx));
            }
            else
            {
                _root.OpenedTabs.Invalidate();
            }

            _root.OpenedTabs.Update();
        }
    }
}