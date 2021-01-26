using System.Windows.Forms;

namespace Editka
{
    public class ColorScheme
    {
        public void ApplyTo(Control control)
        {
            foreach (Control children in control.Controls)
            {
                ApplyTo(children);
            }

            control.ControlAdded -= OnControlAdded;
            control.ControlAdded += OnControlAdded;
            
            var back = control.BackColor;
            var fore = control.ForeColor;
            control.BackColor = fore;
            control.ForeColor = back;
        }

        private void OnControlAdded(object sender, ControlEventArgs e)
        {
            ApplyTo(e.Control);
        }
    }
}