using System;
using System.IO;
using System.Windows.Forms;

namespace Editka.Files
{
    /// <summary>
    /// TODO: Дока
    /// </summary>
    /// <remarks>
    /// Если забыть Dispose — будет плохо. Пользователь будет недоволен. Не надо так.
    /// </remarks>
    public abstract class OpenedFile : BaseNode, IDisposable
    {
        public FileView? Opened;

        /// <summary>
        /// Открывает файл о указанному пути
        /// </summary>
        /// <exception cref="IOException">
        /// Может выкинуть исключение, если не удалось открыть файл!
        /// </exception>
        /// <param name="path">Путь к файлу</param>
        protected OpenedFile(string path) : this()
        {
            File = System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            Path = path;
        }

        protected OpenedFile()
        {
        }

        private FileStream? File { get; set; }
        public abstract FileKind Kind { get; }

        public void Dispose()
        {
            File?.Dispose();
            File = null;
        }

        protected abstract string SuggestedExtension();

        public void FillTextbox(TextboxWrapper textBox)
        {
            textBox.Clear();

            var file = GetFile(false);
            if (file == null)
            {
                return;
            }

            try
            {
                file.Seek(0, SeekOrigin.Begin);
                textBox.LoadFile(file);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool LoadTextbox(TextboxWrapper textBox, bool ask = false)
        {
            var file = GetFile(ask);
            if (file == null)
            {
                return false;
            }

            try
            {
                file.SetLength(0);
                textBox.SaveFile(file);
                file.Flush(true);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private string? AskPath()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = SuggestedExtension(),
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = true,
                Multiselect = false,
                ValidateNames = true
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
            {
                return dialog.FileName;
            }

            return null;
        }

        protected string? GetPath(bool ask)
        {
            if (Path != null)
            {
                return Path;
            }

            if (!ask)
            {
                return null;
            }

            var path = AskPath();
            if (path == null)
            {
                return null;
            }

            Path = path;
            return Path;
        }

        private FileStream? GetFile(bool ask)
        {
            if (File != null)
            {
                return File;
            }

            var path = GetPath(ask);
            if (path == null)
            {
                return null;
            }

            try
            {
                File = System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File = null;
                Path = null;
            }

            return File;
        }
    }
}