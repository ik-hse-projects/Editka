using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Editka.Files
{
    public abstract class BaseNode : TreeNode
    {
        private static int counter;

        public readonly int Id;

        private string? _path;

        public string? Path
        {
            get => _path;
            protected set
            {
                _path = value;
                Filename.Update();
            }
        }

        [XmlIgnore] public readonly Computed<string> Filename;

        public abstract bool IsOpened { get; }

        protected BaseNode()
        {
            Id = counter++;
            Filename = new Computed<string>(() => Path ?? "(untitled)");
            Filename.Changed += (oldValue, newValue) => Text = newValue;
            Text = Filename.Value;
        }

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
        /// <returns>TreeNode, если всё прошло удачно, иначе null.</returns>
        public static BaseNode? AskOpen()
        {
            using var dialog = new OpenFileDialog
            {
                ValidateNames = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
            {
                return Open(dialog.FileName);
            }

            return null;
        }

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