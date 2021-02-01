using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Editka
{
    /// <summary>
    /// Диалог, который указывает на то, что программа компилируется.
    /// </summary>
    public class CompilingDialog : Form
    {
        /// <summary>
        /// Запущенный процесс компилятора.
        /// </summary>
        public readonly Process Process;

        //            (а что ещё может делать конструктор?)
        /// <summary> Создаёт диалог. </summary>
        /// <param name="fileName">Путь к исполняемому файлу.</param>
        /// <param name="workdir">Директория, в которой компилятор будет работать.</param>
        /// <param name="args">Параметры к компилятору. Будут аккуратно экранированы.</param>
        public CompilingDialog(string fileName, string workdir, IEnumerable<string> args) : this(new Process
        {
            StartInfo =
            {
                FileName = fileName,
                // Аккуратное экранирование: https://stackoverflow.com/a/6040946
                Arguments = string.Join(" ", args.Select(arg => "\"" + Regex.Replace(arg, @"(\\+)$", @"$1$1") + "\"")),
                UseShellExecute = false,
                // Я намеренно создаю окошко, т.к. очень сложно сделать нормальное отображение прогресса.
                // А так хоть понятно, не зависло ли эта программа.
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workdir
            },
            EnableRaisingEvents = true
        })
        {
            // (интересно, если перенеосить тело метода в параметры конструктора, то будет ли это считаться <40 строк?)
        }

        private CompilingDialog(Process process)
        {
            Process = process;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            var cancel = new Button
            {
                Text = "Отменить"
            };
            Controls.Add(new FlowLayoutPanel
            {
                Controls =
                {
                    new ProgressBar
                    {
                        Dock = DockStyle.Top,
                        Style = ProgressBarStyle.Marquee
                    },
                    cancel
                }
            });
            cancel.Click += (sender, args) =>
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                    // ignored
                }
            };
            process.Exited += (sender, args) => Invoke((MethodInvoker) (Close));
        }

        /// <summary>
        /// Запускает процесс и отображает диалог.
        /// </summary>
        public void Start()
        {
            try
            {
                Process.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Невозможно запустить процесс: {e}");
                return;
            }

            ShowDialog();
        }
    }
}