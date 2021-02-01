using System;
using Forms = System.Windows.Forms;

namespace Editka.Compat
{
    public class Control
    {
        private Forms.Control _self;

        public Control(Forms.Control self)
        {
            _self = self;
        }

        public Forms.DockStyle Dock
        {
            get => _self.Dock;
            set => _self.Dock = value;
        }
        public Forms.ContextMenu ContextMenu
        {
            get => _self.ContextMenu;
            set => _self.ContextMenu = value;
        }

        public event EventHandler TextChanged
        {
            add => _self.TextChanged += value;
            remove => _self.TextChanged -= value;
        }

        public static implicit operator Forms.Control(Control menu) => menu._self;
        public static implicit operator Control(Forms.Control menu) => new Control(menu);
    }
    
    public static class MainFormExt
    {
        public static void SetContent(this MainForm mainForm, Forms.MainMenu menu, Control control)
        {
            mainForm.Menu = menu;
            mainForm.Controls.Add(control);
        }
    }
}