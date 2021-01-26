using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Editka
{
    public static class NotifyChangedControls
    {
        public static NumericUpDown GetControl(this NotifyChanged<int> self)
        {
            var result = new NumericUpDown
            {
                Value = self.Value,
            };
            result.ValueChanged += (sender, args) => self.Value = (int) result.Value;
            self.Changed += (value, newValue) => result.Value = newValue;
            return result;
        }

        public static CheckBox GetControl(this NotifyChanged<bool> self)
        {
            var result = new CheckBox
            {
                Checked = self.Value,
            };
            result.CheckedChanged += (sender, args) => self.Value = result.Checked;
            self.Changed += (value, newValue) => result.Checked = newValue;
            return result;
        }

        public static Button GetControl(this NotifyChanged<Color> self)
        {
            var result = new Button
            {
                Text = "Выбрать цвет",
                BackColor = self.Value
            };
            result.Click += (sender, args) =>
            {
                var dialog = new ColorDialog
                {
                    Color = self.Value,
                };
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    self.Value = dialog.Color;
                }
            };
            self.Changed += (oldValue, newValue) => result.BackColor = newValue;
            return result;
        }

        public static ComboBox GetControl(this NotifyChanged<Shortcut> self)
        {
            var result = new ComboBox();
            foreach (var res in Enum.GetValues(typeof(Shortcut)))
            {
                result.Items.Add(res);
            }
            
            result.SelectedValueChanged +=
                (sender, args) => self.Value = (result.SelectedItem as Shortcut?) ?? Shortcut.None;
            self.Changed += (oldValue, newValue) => result.SelectedItem = newValue;
            result.SelectedItem = self.Value;
            return result;
        }
    }
}