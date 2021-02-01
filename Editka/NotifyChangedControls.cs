using System;
using System.Windows.Forms;

namespace Editka
{
    public static class NotifyChangedControls
    {
        // FIXME: Здесь должны быть слабые ссылки, иначе получается утечка памяти.
        // Созданные контролы никогда не уничтожаются, поскольку есть вероятность, что их потребуется обновить.

        public static NumericUpDown GetControl(this NotifyChanged<int> self)
        {
            var result = new NumericUpDown
            {
                Value = self.Value
            };
            result.ValueChanged += (sender, args) => self.Value = (int) result.Value;
            self.Changed += (value, newValue) => result.Value = newValue;
            return result;
        }

        public static CheckBox GetControl(this NotifyChanged<bool> self)
        {
            var result = new CheckBox
            {
                Checked = self.Value
            };
            result.CheckedChanged += (sender, args) => self.Value = result.Checked;
            self.Changed += (value, newValue) => result.Checked = newValue;
            return result;
        }

        public static Button GetControl(this NotifyChanged<SerializableColor> self)
        {
            var button = new Button
            {
                Text = "Выбрать цвет",
                BackColor = self.Value.Color
            };
            button.Click += (sender, args) =>
            {
                var dialog = new ColorDialog
                {
                    Color = self.Value.Color
                };
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var changed = self.Value;
                    changed.Color = dialog.Color;
                    self.Value = changed;
                }
            };
            self.Changed += (oldValue, newValue) => button.BackColor = newValue.Color;
            return button;
        }

        public static ComboBox GetControl(this NotifyChanged<Shortcut> self)
        {
            var result = new ComboBox();
            foreach (var res in Enum.GetValues(typeof(Shortcut)))
            {
                result.Items.Add(res);
            }

            result.SelectedValueChanged +=
                (sender, args) => self.Value = result.SelectedItem as Shortcut? ?? Shortcut.None;
            self.Changed += (oldValue, newValue) => result.SelectedItem = newValue;
            result.SelectedItem = self.Value;
            return result;
        }

        public static TextBox GetControl(this NotifyChanged<string?> self)
        {
            var result = new TextBox {Text = self.Value};
            result.TextChanged += (sender, args) => self.Value = result.Text;
            self.Changed += (value, newValue) => result.Text = newValue;
            return result;
        }
    }
}