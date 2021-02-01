using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Editka
{
    /// <summary>
    /// Лог компиляции.
    /// </summary>
    public class BuildLog : TabPage
    {
        private readonly TextBox _output;
        private readonly MainForm _root;

        public BuildLog(MainForm root) : base("Компиляция")
        {
            Dock = DockStyle.Fill;
            _output = new TextBox
            {
                Font = new Font(FontFamily.GenericMonospace, 12),
                Dock = DockStyle.Fill,
                Multiline = true
            };
            _root = root;
            Controls.Add(_output);
        }

        /// <summary>
        /// Отображает лог пользователю.
        /// </summary>
        private void SetLog(string output)
        {
            _output.Text = output;
        }

        /// <summary>
        /// Запукскает компиляцию.
        /// </summary>
        /// <param name="path">Путь к файлу, который надо собрать.</param>
        /// <param name="builder">Имя компилятора: csc или dotnet.</param>
        public void DoBuild(string path, string builder)
        {
            string? compiler;
            while (true)
            {
                compiler = builder switch
                {
                    "dotnet" => _root.Settings.DotnetPath.Value,
                    "csc" => _root.Settings.CscPath.Value,
                    _ => null
                };
                if (compiler != null)
                {
                    break;
                }

                if (!new SettingsDialog(_root).AskOpen($"Требуется указать путь к {builder}.exe"))
                {
                    return;
                }
            }

            var args = builder switch
            {
                "dotnet" => new[] {"build", path},
                "csc" => new[] {path},
                _ => new string[0]
            };

            var dialog = new CompilingDialog(compiler, Path.GetDirectoryName(path), args);
            dialog.Start();
            string output;
            try
            {
                output = dialog.Process.StandardOutput.ReadToEnd();
            }
            catch
            {
                output = "(вывода нет)";
            }

            SetLog(output);
        }
    }
}