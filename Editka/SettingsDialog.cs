using System.Drawing;
using System.Windows.Forms;

namespace Editka
{
    /// <summary>
    /// Диалог настроек.
    /// </summary>
    public class SettingsDialog : Form
    {
        public SettingsDialog(MainForm root)
        {
            AutoSize = true;
            var close = new Button
            {
                Text = "Закрыть"
            };
            close.Click += (sender, args) => Close();

            var hotkeys = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Location = new Point(5, 15),
                MaximumSize = new Size(800, 10000),
                FlowDirection = FlowDirection.LeftToRight,
                Width = Width
            };
            SizeChanged += (sender, args) => hotkeys.Width = Width;
            foreach (var pair in root.Settings.Hotkeys.Notifiable())
            {
                hotkeys.Controls.Add(new TableLayoutPanel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    // https://stackoverflow.com/a/30019805
                    BorderStyle = BorderStyle.FixedSingle,
                    ColumnCount = 2,
                    Controls =
                    {
                        pair.Value.GetControl(),
                        new Label
                        {
                            Text = pair.Key,
                            AutoSize = true
                        }
                    }
                });
            }

            Controls.Add(new FlowLayoutPanel
            {
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Controls =
                {
                    new GroupBox
                    {
                        Text = "Сохранение",
                        AutoSize = true,
                        Controls =
                        {
                            new TableLayoutPanel
                            {
                                AutoSize = true,
                                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                                Location = new Point(5, 15),
                                Controls =
                                {
                                    {root.Settings.AutosaveSeconds.GetControl(), 1, 1},
                                    {new Label {Text = "Интервал автосохранения", AutoSize = true}, 2, 1}
                                }
                            }
                        }
                    },
                    new GroupBox
                    {
                        Text = "Цвета",
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        Controls =
                        {
                            new TableLayoutPanel
                            {
                                AutoSize = true,
                                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                                Location = new Point(5, 15),
                                Controls =
                                {
                                    {root.Settings.Colors.Get("background").GetControl(), 1, 1},
                                    {new Label {Text = "Фон", AutoSize = true}, 2, 1},
                                    {root.Settings.Colors.Get("foreground").GetControl(), 1, 2},
                                    {new Label {Text = "Передний план", AutoSize = true}, 2, 2}
                                }
                            }
                        }
                    },
                    new GroupBox
                    {
                        Text = "Компиляция",
                        AutoSize = true,
                        Controls =
                        {
                            new TableLayoutPanel
                            {
                                AutoSize = true,
                                Location = new Point(5, 15),
                                Controls =
                                {
                                    {root.Settings.CscPath.GetControl(), 1, 1},
                                    {new Label {Text = "csc.exe", AutoSize = true}, 2, 1},
                                    {root.Settings.DotnetPath.GetControl(), 1, 2},
                                    {new Label {Text = "dotnet.exe", AutoSize = true}, 2, 2}
                                }
                            }
                        }
                    },
                    new GroupBox
                    {
                        Text = "Горячие клавиши",
                        AutoSize = true,
                        Controls = {hotkeys}
                    },
                    close
                }
            });
        }

        /// <summary>
        /// Создаёт вопрос с преложением установить некоторый параметр.
        /// Если пользователь соглашается, то открывает диалог настроек и блокируется до закрытия.
        /// </summary>
        /// <param name="message">Сообщение к пользователю.</param>
        /// <returns>true, если пользователь согласился.</returns>
        public bool AskOpen(string message)
        {
            var result = MessageBox.Show(message, "Требуется настройка", MessageBoxButtons.OKCancel);
            if (result != DialogResult.OK)
            {
                return false;
            }

            ShowDialog();
            return true;
        }
    }
}