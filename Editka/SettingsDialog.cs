using System.Drawing;
using System.Windows.Forms;

namespace Editka
{
    public class SettingsDialog : Form
    {
        public SettingsDialog(MainForm root)
        {
            AutoSize = true;
            Controls.Add(new GroupBox
            {
                Text = "Сохранение",
                AutoSize = true,
                Location = new Point(5, 5),
                Controls =
                {
                    new TableLayoutPanel
                    {
                        AutoSize = true,
                        ColumnCount = 2,
                        RowCount = 3,
                        Location = new Point(5, 20),
                        ColumnStyles =
                        {
                            new ColumnStyle(SizeType.Absolute, 10),
                            new ColumnStyle(SizeType.AutoSize)
                        },
                        RowStyles =
                        {
                            new RowStyle(SizeType.AutoSize),
                            new RowStyle(SizeType.AutoSize),
                            new RowStyle(SizeType.AutoSize),
                        },
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
                                    Text = "Включить журналирование (работает лучше всего в паре с автосохранением)",
                                    AutoSize = true
                                },
                                2, 3
                            },
                        }
                    }
                }
            });
        }
    }
}