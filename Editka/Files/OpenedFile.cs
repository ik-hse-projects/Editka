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
    public class OpenedFile : TreeNode, IDisposable
    {
        public FileStream? File { get; }
        public string? Path { get; }

        public string Filename => Path ?? "(untitled)";

        /// <summary>
        /// Открывает файл о указанному пути
        /// </summary>
        /// <exception cref="IOException">
        /// Может выкинуть исключение, если не удалось открыть файл!
        /// </exception>
        /// <param name="path">Путь к файлу</param>
        public OpenedFile(string path)
        {
            File = System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            Path = path;
            Text = Filename;
        }

        public OpenedFile()
        {
            Text = Filename;
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
            if (result == DialogResult.OK)
            {
                try
                {
                    return new OpenedFile(dialog.FileName);
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