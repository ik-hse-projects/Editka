using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

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

        public FileView? Opened;

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

        [XmlIgnore] public readonly Computed<string> Filename;

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
            Id = counter++;
            Filename = new Computed<string>(() => Path ?? "(untitled)");
            Filename.Changed += (oldValue, newValue) => Text = newValue;
            Text = Filename.Value;
        }

        protected abstract string SuggestedExtension();
        protected abstract RichTextBoxStreamType StreamType { get; }

        public void FillTextbox(RichTextBox textBox)
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
                textBox.LoadFile(file, StreamType);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool LoadTextbox(RichTextBox textBox, bool ask = false)
        {
            var file = GetFile(ask);
            if (file == null)
            {
                return false;
            }

            try
            {
                file.SetLength(0);
                textBox.SaveFile(File, StreamType);
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

        private FileStream? GetFile(bool ask)
        {
            if (File != null)
            {
                return File;
            }

            if (Path == null)
            {
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
            }

            try
            {
                File = System.IO.File.Open(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File = null;
                Path = null;
            }

            return File;
        }

        public static OpenedFile? Open(string path)
        {
            var extension = System.IO.Path.GetExtension(path);
            try
            {
                return extension switch
                {
                    ".rtf" => new Rich(path),
                    ".cs" => new CSharp(path),
                    _ => new Plain(path)
                };
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Невозможно открыть файл", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return null;
            }
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
                return Open(dialog.FileName);
            }

            return null;
        }

        public void Dispose()
        {
            File?.Dispose();
            File = null;
        }
    }
}