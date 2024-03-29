using System;
using System.ComponentModel;
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
            _root.Close();
        }

        public void Exit(object sender, CancelEventArgs eventArgs)
        {
            if (!Exit())
            {
                eventArgs.Cancel = true;
            }
            else
            {
                _root.State.Serialize(_root);
            }
        }

        public bool Exit()
        {
            if (_root.OpenedTabs.FileTabs.All(tab => !tab.Changed.Value))
            {
                return true;
            }

            var dialog = MessageBox.Show("Сохранить все изменения?", "Выход", MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);
            switch (dialog)
            {
                case DialogResult.Yes:
                    SaveAll();
                    return true;
                case DialogResult.No:
                    return true;
                default:
                    return false;
            }
        }

        public void New(object sender, EventArgs e)
        {
            New();
        }

        public void New()
        {
            var dialog = MessageBox.Show("Использовать возможности RTF? Иначе будет создан обычный текстовый файл.",
                "Новый файл", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            var file = dialog == DialogResult.Yes ? (OpenedFile) new Rich() : new Plain();
            _root.FileList.TreeView.Nodes.Add(file);
            _root.OpenedTabs.Open(file);
        }

        public void Open(object sender, EventArgs e)
        {
            Open();
        }

        public void Open()
        {
            var file = BaseNode.AskOpen();
            if (file != null)
            {
                _root.FileList.TreeView.Nodes.Add(file);
                if (file is OpenedFile openedFile)
                {
                    _root.OpenedTabs.Open(openedFile);
                }
            }
        }

        public void OpenDirectory(object sender, EventArgs e)
        {
            var dir = BaseNode.AskDirectory();
            if (dir != null)
            {
                _root.FileList.TreeView.Nodes.Add(dir);
            }
        }

        public void OpenInNewWindow(object sender, EventArgs eventArgs)
        {
            var window = NewWindow();
            window.Actions.Open();
        }

        public void CreateInNewWindow(object sender, EventArgs eventArgs)
        {
            var window = NewWindow();
            window.Actions.New();
        }

        public void SaveAll(object sender, EventArgs e)
        {
            SaveAll();
        }

        public void SaveAll(bool ask = true)
        {
            foreach (TabPage tabPage in _root.OpenedTabs.TabPages)
            {
                if (tabPage is FileView fileView)
                {
                    fileView.Save(ask);
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
            if (!(_root.CurrentFile?.File is CSharp cSharp))
            {
                MessageBox.Show("Этот файл нельзя скомпилировать этой программой.");
                return;
            }

            if (cSharp.Solution != null)
            {
                cSharp.Solution.Build(_root);
            }
            else
            {
                cSharp.Build(_root);
            }
        }

        public void Settings(object sender, EventArgs e)
        {
            new SettingsDialog(_root).ShowDialog(_root);
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

        public void Bold(object sender, EventArgs e)
        {
            _root.CurrentFile?.TextBox.UpdateStyle(FontStyle.Bold);
        }

        public void Cursive(object sender, EventArgs e)
        {
            _root.CurrentFile?.TextBox.UpdateStyle(FontStyle.Italic);
        }

        public void Underline(object sender, EventArgs e)
        {
            _root.CurrentFile?.TextBox.UpdateStyle(FontStyle.Underline);
        }

        public void Strikethrough(object sender, EventArgs e)
        {
            _root.CurrentFile?.TextBox.UpdateStyle(FontStyle.Strikeout);
        }

        public void NewWindow(object sender, EventArgs eventArgs)
        {
            NewWindow();
        }

        public MainForm NewWindow()
        {
            var window = new MainForm(_root.State.CloneSettings());
            MultiFormContext.Context.AddForm(window);
            return window;
        }
    }
}