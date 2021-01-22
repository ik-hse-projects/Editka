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
    public abstract class OpenedFile : TreeNode, IDisposable
    {
        private static int counter;
        public readonly int Id;
        
        private string? _path;
        public FileStream? File { get; private set; }

        public string? Path
        {
            get => _path;
            private set
            {
                _path = value;
                Filename.Update();
            }
        }

        public Computed<string> Filename;

        /// <summary>
        /// Открывает файл о указанному пути
        /// </summary>
        /// <exception cref="IOException">
        /// Может выкинуть исключение, если не удалось открыть файл!
        /// </exception>
        /// <param name="path">Путь к файлу</param>
        protected OpenedFile(string path): this()
        {
            File = System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            Path = path;
        }

        protected OpenedFile()
        {
            Id = counter++;
            Filename = new Computed<string>(() => Path ?? "(untitled)");
            Filename.Changed += (oldValue, newValue) => Text = newValue;
            Text = Filename.Value;
        }

        protected abstract RichTextBoxStreamType StreamType { get; }

        public void FillTextbox(RichTextBox textBox)
        {
            textBox.Clear();

            if (File == null)
            {
                return;
            }

            try
            {
                File.Seek(0, SeekOrigin.Begin);
                textBox.LoadFile(File, StreamType);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadTextbox(RichTextBox textBox)
        {
            var file = GetFile(".txt");
            if (file == null)
            {
                return;
            }

            try
            {
                file.SetLength(0);
                textBox.SaveFile(File, StreamType);
                file.Flush(true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private FileStream? GetFile(string extension)
        {
            if (File != null)
            {
                return File;
            }

            var dialog = new OpenFileDialog
            {
                DefaultExt = extension,
                CheckFileExists = false,
                CheckPathExists = true,
                Multiselect = false,
                ValidateNames = true
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
            {
                var path = dialog.FileName;
                try
                {
                    File = System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                    Path = path;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    File = null;
                    Path = null;
                }
            }

            return File;
        }

        /// <summary>
        /// При помощи диалога запрашивает у пользователя путь к файлу и пытается его открыть.
        /// </summary>
        /// <returns>OpenedFile, если всё прошло удачно, иначе null.</returns>
        public static OpenedFile? AskOpen()
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                Multiselect = false,
                ValidateNames = true
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
            {
                var extension = System.IO.Path.GetExtension(dialog.FileName);
                try
                {
                    return extension switch
                    {
                        ".rtf" => new Rich(dialog.FileName),
                        _ => new Plain(dialog.FileName)
                    };
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Невозможно открыть файл", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            return null;
        }

        public void Dispose()
        {
            File?.Dispose();
        }
    }
}