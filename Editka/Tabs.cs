using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Editka.Files;

namespace Editka
{
    public class Tabs : TabControl
    {
        private MainForm _root;
        public IEnumerable<FileView> FileTabs => TabPages.OfType<FileView>();

        public new FileView? SelectedTab
        {
            get => ((TabControl) this).SelectedTab as FileView;
            set => ((TabControl) this).SelectedTab = value;
        }

        public void Open(OpenedFile openedFile)
        {
            if (openedFile.Opened == null)
            {
                var page = new FileView(_root, openedFile);
                TabPages.Add(page);
                openedFile.Opened = page;
            }

            SelectedTab = openedFile.Opened;
        }

        public Tabs(MainForm root)
        {
            _root = root;

            // https://stackoverflow.com/q/47175493
            MouseClick += OnMouseEventHandler;
        }

        private void OnMouseEventHandler(object sender, MouseEventArgs click)
        {
            if (click.Button != MouseButtons.Right)
            {
                return;
            }

            for (var i = 0; i < TabPages.Count; i++)
            {
                if (GetTabRect(i).Contains(click.Location))
                {
                    if (TabPages[i] is FileView fileView && fileView.Changed.Value)
                    {
                        var dialog = MessageBox.Show("Сохранить изменения?", "Закрыть вкладку", MessageBoxButtons.YesNoCancel);
                        switch (dialog)
                        {
                            case DialogResult.Yes:
                                fileView.Save();
                                break;
                            case DialogResult.Cancel:
                                return;
                        }

                        fileView.Close();
                    }

                    TabPages.RemoveAt(i);
                    return;
                }
            }
        }
    }
}