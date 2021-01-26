using System.Drawing;
using System.Windows.Forms;

namespace Editka
{
    public class SettingsDialog : Form
    {
        public SettingsDialog(MainForm root)
        {
            AutoSize = true;
            var close = new Button
            {
                Text = "Закрыть",
            };
            close.Click += (sender, args) => Close();

            Controls.Add(new FlowLayoutPanel
            {
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Dock = DockStyle.Fill,
                AutoSize = true,
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
                                Location = new Point(5, 15),
                                Controls =
                                {
                                    {root.Settings.AutosaveSeconds.GetControl(), 1, 1},
                                    {new Label {Text = "Интервал автосохранения", AutoSize = true}, 2, 1},
                                    {root.Settings.SaveOnFocus.GetControl(), 1, 2},
                                    {new Label {Text = "Сохранять при смене фокуса", AutoSize = true}, 2, 2},
                                    {root.Settings.EnableHistory.GetControl(), 1, 3},
                                    {
                                        new Label
                                        {
                                            Text =
                                                "Включить журналирование (работает лучше всего в паре с автосохранением)",
                                            AutoSize = true
                                        },
                                        2, 3
                                    },
                                }
                            }
                        }
                    },
                    new GroupBox
                    {
                        Text = "Цвета",
                        AutoSize = true,
                        Controls =
                        {
                            new TableLayoutPanel
                            {
                                AutoSize = true,
                                Location = new Point(5, 15),
                                Controls =
                                {
                                    {root.Settings.Colors.Get("background").GetControl(), 1, 1},
                                    {new Label {Text = "Фон", AutoSize = true}, 2, 1},
                                    {root.Settings.Colors.Get("foreground").GetControl(), 1, 2},
                                    {new Label {Text = "Передний план", AutoSize = true}, 2, 2},
                                }
                            }
                        }
                    },
                    close
                }
            });
        }
    }
}