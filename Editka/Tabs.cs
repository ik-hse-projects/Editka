using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Editka.Files;

namespace Editka
{
    /// <summary>
    /// Открытые вкладки.
    /// </summary>
    public class Tabs : TabControl
    {
        private readonly MainForm _root;

        public Tabs(MainForm root)
        {
            _root = root;

            MouseClick += MaybeClose;
        }

        /// <summary>
        /// Вкладки с файлами.
        /// </summary>
        public IEnumerable<FileView> FileTabs => TabPages.OfType<FileView>();

        /// <summary>
        /// Выбранная на данный момент вкладка.
        /// </summary>
        public new FileView? SelectedTab
        {
            get => ((TabControl) this).SelectedTab as FileView;
            set => ((TabControl) this).SelectedTab = value;
        }

        /// <summary>
        /// Открывает файл в новой вкладке, либо фокусирвется на существующую.
        /// </summary>
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

        /// <summary>
        /// Обработчик правого клика по вкладке.
        /// </summary>
        private void MaybeClose(object sender, MouseEventArgs click)
        {
            if (click.Button != MouseButtons.Right)
            {
                return;
            }

            for (var i = 0; i < TabPages.Count; i++)
            {
                // https://stackoverflow.com/q/47175493
                if (GetTabRect(i).Contains(click.Location))
                {
                    if (TabPages[i] is FileView fileView)
                    {
                        if (fileView.Changed.Value)
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
                        }

                        fileView.Close();
                    }
                    else
                    {
                        TabPages.RemoveAt(i);
                    }

                    return;
                }
            }
        }
    }
}