using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Forms = System.Windows.Forms;

namespace Editka.Compat
{
    public class ContextMenu
    {
        private ContextMenuStrip _self;

        public ContextMenu(ContextMenuStrip self)
        {
            _self = self;
        }

        public ContextMenu() : this(new ContextMenuStrip())
        {
        }

        public static implicit operator ContextMenuStrip(ContextMenu menu) => menu._self;
        public static implicit operator ContextMenu(ContextMenuStrip menu) => new ContextMenu(menu);

        public Menu.MenuItemCollection MenuItems => _self.Items;
    }

    public class MainMenu
    {
        private MenuStrip _self;

        public MainMenu() : this(new MenuStrip())
        {
        }

        public MainMenu(MenuStrip self)
        {
            _self = self;
        }

        public static implicit operator MenuStrip(MainMenu menu) => menu._self;
        public static implicit operator MainMenu(MenuStrip menu) => new MainMenu(menu);

        public Menu.MenuItemCollection MenuItems => _self.Items;
    }

    // https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/Shortcut.cs,0a5db68c72ba90b7
    public static class ShortcutConversionExt
    {
        public static Keys ToKeys(this Shortcut self)
        {
            return (Keys)(long)self;
        }

        public static Shortcut ToShortcut(this Keys self)
        {
            return (Shortcut)(long)self;
        }
    }

    public class MenuItem
    {
        private ToolStripMenuItem _self;

        internal MenuItem(ToolStripMenuItem self)
        {
            _self = self;
        }

        public MenuItem(string text) : this(new ToolStripMenuItem(text))
        {
        }

        public MenuItem(string text, EventHandler click) : this(text)
        {
            _self.Click += click;
        }

        public bool ShowShortcut
        {
            get => _self.ShowShortcutKeys;
            set => _self.ShowShortcutKeys = value;
        }

        public Shortcut Shortcut
        {
            get => _self.ShortcutKeys.ToShortcut();
            set => _self.ShortcutKeys = value.ToKeys();
        }

        public Menu.MenuItemCollection MenuItems => _self.DropDownItems;

        public static implicit operator ToolStripMenuItem(MenuItem menuItem) => menuItem._self;
        public static implicit operator MenuItem(ToolStripMenuItem menuItem) => new MenuItem(menuItem);
    }

    public class Menu
    {
        public class MenuItemCollection
        {
            private ToolStripItemCollection _self;

            public MenuItemCollection(ToolStripItemCollection self)
            {
                _self = self;
            }

            public MenuItemCollection(ToolStrip owner, ToolStripItem[] value) : this(new ToolStripItemCollection(owner, value))
            {
            }

            public static implicit operator ToolStripItemCollection(MenuItemCollection menuItem) => menuItem._self;
            public static implicit operator MenuItemCollection(ToolStripItemCollection itemCollection) => new MenuItemCollection(itemCollection);

            public MenuItem Add(string name, EventHandler click)
            {
                var item = new MenuItem(name, click);
                _self.Add(item);
                return (MenuItem)(ToolStripItem)item;
            }

            public void Add(ToolStripItem item)
            {
                _self.Add(item);
            }

            public void AddRange(IEnumerable<MenuItem> items)
            {
                foreach (var i in items)
                {
                    Add(i);
                }
            }
        }
    }

    public class Control
    {
        private Forms.Control _self;

        public Control(Forms.Control self)
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
            get => _self.ContextMenuStrip;
            set => _self.ContextMenuStrip = value;
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
        public static void SetContent(this MainForm mainForm, MainMenu menu, Control control)
        {
            mainForm.MainMenuStrip = menu;
            mainForm.Controls.Add(new Forms.TableLayoutPanel
            {
                Dock = Forms.DockStyle.Fill,
                ColumnCount = 1,
                Controls =
                {
                    menu,
                    control
                }
            });
        }
    }
}
