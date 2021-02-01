using System.Windows.Forms;
using Editka.Files;

namespace Editka
{
    /// <summary>
    /// Класс, отвечающий за отображение файла.
    /// </summary>
    public class FileView : TabPage
    {
        private readonly MainForm _root;

        /// <summary>
        /// Изменён ли файл?
        /// </summary>
        public readonly NotifyChanged<bool> Changed = new NotifyChanged<bool>(false);

        /// <summary>
        /// Содержимое файла и другая важная информаиця про него.
        /// </summary>
        public readonly OpenedFile File;

        /// <summary>
        /// Тот Control, который собственно содержит текст.
        /// </summary>
        public readonly TextboxWrapper TextBox;

        public FileView(MainForm root, OpenedFile openedFile)
        {
            File = openedFile;
            _root = root;

            TextBox = new TextboxWrapper(File.Kind);
            TextBox.Control.Dock = DockStyle.Fill;
            TextBox.Control.ContextMenu = MenuCreator.ContextMenu(_root);
            File.FillTextbox(TextBox);
            Controls.Add(TextBox.Control);

            openedFile.Opened = this;

            TextBox.Control.TextChanged += (sender, args) => Changed.Value = true;
            Changed.Changed += (oldValue, newValue) => UpdateText();
            openedFile.Filename.Changed += (oldValue, newValue) => UpdateText();
            UpdateText();
        }

        /// <summary>
        /// Сохраняет файл, если это нужно.
        /// </summary>
        /// <param name="ask">Следует ли спрашивать путь к файлу, если это новый файл?</param>
        public void Save(bool ask = true)
        {
            if (!Changed.Value)
            {
                return;
            }

            if (File.LoadTextbox(TextBox, ask))
            {
                Changed.Value = false;
            }
        }

        /// <summary>
        /// Обновляет имя вкладки.
        /// </summary>
        private void UpdateText()
        {
            Text = Changed.Value ? $"{File.Filename.Value}*" : File.Filename.Value;
            _root.OpenedTabs.Invalidate();
            _root.OpenedTabs.Update();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                File.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Закрывает вкладку и освобождает открытый файл.
        /// </summary>
        public void Close()
        {
            File.Opened = null;
            _root.OpenedTabs.TabPages.Remove(this);
            
            Dispose();
        }
    }
}