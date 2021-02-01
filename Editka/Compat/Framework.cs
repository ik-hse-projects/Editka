using System;
using System.Windows.Forms;

namespace Editka.Compat
{
    public class Control
    {
        private readonly System.Windows.Forms.Control _self;

        public Control(System.Windows.Forms.Control self)
        {
            _self = self;
        }

        public DockStyle Dock
        {
            get => _self.Dock;
            set => _self.Dock = value;
        }

        public ContextMenu ContextMenu
        {
            get => _self.ContextMenu;
            set => _self.ContextMenu = value;
        }

        public event EventHandler TextChanged
        {
            add => _self.TextChanged += value;
            remove => _self.TextChanged -= value;
        }

        public static implicit operator System.Windows.Forms.Control(Control menu)
        {
            return menu._self;
        }

        public static implicit operator Control(System.Windows.Forms.Control menu)
        {
            return new Control(menu);
        }
    }

    public static class MainFormExt
    {
        public static void SetContent(this MainForm mainForm, MainMenu menu, Control control)
        {
            mainForm.Menu = menu;
            mainForm.Controls.Add(control);
        }
    }
}