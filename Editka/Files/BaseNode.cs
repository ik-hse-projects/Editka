using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Editka.Files
{
    /// <summary>
    /// Узел дерева файлов: файл или директория.
    /// </summary>
    public abstract class BaseNode : TreeNode
    {
        /// <summary>
        /// Имя файла.
        /// </summary>
        public readonly Computed<string> Filename;

        /// <summary>
        /// backing field для <see cref="Path"/>.
        /// </summary>
        private string? _path;

        protected BaseNode()
        {
            Filename = new Computed<string>(() => Path ?? "(untitled)");
            Filename.Changed += (oldValue, newValue) => Text = newValue;
            Text = Filename.Value;
        }

        /// <summary>
        /// Путь к файлу/директории, если он вообще есть.
        /// </summary>
        public string? Path
        {
            get => _path;
            protected set
            {
                _path = value;
                Filename.Update();
            }
        }

        /// <summary>
        /// Проверка, поддерживается ли расширение программой.
        /// </summary>
        protected static bool IsKnownExtension(string extension)
        {
            return extension switch
            {
                ".txt" => true,
                ".rtf" => true,
                ".cs" => true,
                _ => false
            };
        }

        /// <summary>
        /// Открывает файл/директорию. Если происзходит ошибка, то возвращает null.
        /// </summary>
        /// <param name="silent">Если true, то не будут выводиться никакие сообщения пользователю.</param>
        public static BaseNode? Open(string path, bool silent = false)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    return new OpenedDirectory(path);
                }

                var extension = System.IO.Path.GetExtension(path);
                return extension switch
                {
                    ".rtf" => new Rich(path),
                    ".cs" => new CSharp(path),
                    ".txt" => new Plain(path),
                    ".sln" => new Solution(path),
                    ".csproj" => new Solution(path),
                    _ => null
                };
            }
            catch (Exception e)
            {
                if (!silent)
                {
                    MessageBox.Show(e.ToString(), "Невозможно открыть файл", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                return null;
            }
        }

        /// <summary>
        /// При помощи диалога запрашивает у пользователя путь к файлу и пытается его открыть.
        /// </summary>
        /// <returns>BaseNode, если всё прошло удачно, иначе null.</returns>
        public static BaseNode? AskOpen()
        {
            using var dialog = new OpenFileDialog
            {
                ValidateNames = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
            {
                return Open(dialog.FileName);
            }

            return null;
        }

        /// <summary>
        /// Спршивает у пользователю путь к директории и открывает её.
        /// </summary>
        public static BaseNode? AskDirectory()
        {
            using var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                return Open(dialog.SelectedPath);
            }

            return null;
        }
    }
}