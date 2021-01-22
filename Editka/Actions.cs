using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Editka.Files;

namespace Editka
{
    public class Actions
    {
        private readonly MainForm _root;

        public Actions(MainForm root)
        {
            _root = root;
        }

        public void Exit(object sender, EventArgs eventArgs)
        {
            if (_root.OpenedTabs.FileTabs.All(tab => !tab.Changed.Value))
            {
                Environment.Exit(0);
            }

            var dialog = MessageBox.Show("Сохранить все изменения?", "Выход", MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);
            switch (dialog)
            {
                case DialogResult.Yes:
                    SaveAll(sender, eventArgs);
                    Environment.Exit(0);
                    break;
                case DialogResult.No:
                    Environment.Exit(0);
                    break;
            }
        }

        public void New(object sender, EventArgs e)
        {
            var dialog = MessageBox.Show("Использовать возможности RTF? Иначе будет создан обычный текстовый файл.",
                "Новый файл", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                _root.FileList.Nodes.Add(new Rich());
            }
            else
            {
                _root.FileList.Nodes.Add(new Plain());
            }
        }

        public void Open(object sender, EventArgs e)
        {
            var file = OpenedFile.AskOpen();
            if (file != null)
            {
                _root.FileList.Nodes.Add(file);
            }
        }

        public void SaveAll(object sender, EventArgs e)
        {
            foreach (TabPage tabPage in _root.OpenedTabs.TabPages)
            {
                if (tabPage is FileView fileView)
                {
                    fileView.Save();
                }
            }
        }

        public void Save(object sender, EventArgs e)
        {
            _root.CurrentFile?.Save();
        }

        public void Undo(object sender, EventArgs e)
        {
            _root.CurrentFile?.TextBox.Undo();
        }

        public void Redo(object sender, EventArgs e)
        {
            _root.CurrentFile?.TextBox.Redo();
        }

        public void Build(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Run(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Settings(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void SelectAll(object sender, EventArgs e)
        {
            _root.CurrentFile?.TextBox.SelectAll();
        }

        public void Cut(object sender, EventArgs e)
        {
            _root.CurrentFile?.TextBox.Cut();
        }

        public void Copy(object sender, EventArgs e)
        {
            _root.CurrentFile?.TextBox.Copy();
        }

        public void Paste(object sender, EventArgs e)
        {
            _root.CurrentFile?.TextBox.Paste();
        }

        public void Format(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UpdateStyle(FontStyle mask)
        {
            if (_root.CurrentFile == null)
            {
                return;
            }

            if (!(_root.CurrentFile.File is Rich))
            {
                MessageBox.Show("Нельзя использовать форматирование в обычных файлах");
                return;
            }

            var font = _root.CurrentFile.TextBox.SelectionFont;
            if (font == null)
            {
                return;
            }

            // Напоминаю, что XOR с единицей инвертирует бит, т.е. переключает стиль.
            var newStyle = font.Style ^ mask;
            _root.CurrentFile.TextBox.SelectionFont = new Font(font.FontFamily, font.Size, newStyle);
        }

        public void Bold(object sender, EventArgs e)
        {
            UpdateStyle(FontStyle.Bold);
        }

        public void Cursive(object sender, EventArgs e)
        {
            UpdateStyle(FontStyle.Italic);
        }

        public void Underline(object sender, EventArgs e)
        {
            UpdateStyle(FontStyle.Underline);
        }

        public void Strikethrough(object sender, EventArgs e)
        {
            UpdateStyle(FontStyle.Strikeout);
        }
    }
}