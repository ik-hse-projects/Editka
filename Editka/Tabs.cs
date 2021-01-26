using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Editka
{
    public class Tabs : TabControl
    {
        public IEnumerable<FileView> FileTabs => TabPages.OfType<FileView>();

        public new FileView? SelectedTab
        {
            get => ((TabControl) this).SelectedTab as FileView;
            set => ((TabControl) this).SelectedTab = value;
        }

        public Tabs()
        {
            // https://stackoverflow.com/q/47175493
            MouseClick += (sender, click) =>
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
                            var dialog = MessageBox.Show("Сохранить изменения?", "Закрыть вкладку",
                                MessageBoxButtons.YesNoCancel);
                            switch (dialog)
                            {
                                case DialogResult.Yes:
                                    fileView.Save();
                                    break;
                                case DialogResult.Cancel:
                                    return;
                            }
                            fileView.Dispose();
                        }

                        TabPages.RemoveAt(i);
                        return;
                    }
                }
            };
        }
    }
}