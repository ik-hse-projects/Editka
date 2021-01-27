using System.Windows.Forms;

namespace Editka
{
    public class ColorScheme
    {
        private readonly MainForm _root;

        public ColorScheme(MainForm root)
        {
            _root = root;
            _root.Settings.Colors.Get("foreground").Changed += (_, __) => ApplyTo(_root);
            _root.Settings.Colors.Get("background").Changed += (_, __) => ApplyTo(_root);
        }

        public void ApplyTo(Control control)
        {
            foreach (Control children in control.Controls)
            {
                ApplyTo(children);
            }

            control.ControlAdded -= OnControlAdded;
            control.ControlAdded += OnControlAdded;

            control.BackColor = _root.Settings.Colors.Get("background").Value.Color;
            control.ForeColor = _root.Settings.Colors.Get("foreground").Value.Color;
        }

        private void OnControlAdded(object sender, ControlEventArgs e)
        {
            ApplyTo(e.Control);
        }
    }
}