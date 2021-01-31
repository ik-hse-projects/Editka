using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Editka
{
    public class CompilingDialog : Form
    {
        public Process Process;

        public CompilingDialog(string fileName, string workdir, IEnumerable<string> args) : this(new Process
        {
            StartInfo =
            {
                FileName = fileName,
                // https://stackoverflow.com/a/6040946
                Arguments = string.Join(" ", args.Select(arg => "\"" + Regex.Replace(arg, @"(\\+)$", @"$1$1") + "\"")),
                UseShellExecute = false,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workdir,
            },
            EnableRaisingEvents = true
        })
        {
        }

        public CompilingDialog(Process process)
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
                        Style = ProgressBarStyle.Marquee,
                    },
                    cancel
                }
            });
            cancel.Click += (sender, args) => process.Kill();
            process.Exited += (sender, args) => Close();
            process.Start();
        }
    }
}