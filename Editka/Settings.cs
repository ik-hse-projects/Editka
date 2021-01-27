using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Editka
{
    public class Settings
    {
        [XmlIgnore] private readonly MainForm _root;

        public readonly NotifiableDictionary<string, Color> Colors = new NotifiableDictionary<string, Color>();
        public readonly NotifyChanged<int> AutosaveSeconds = new NotifyChanged<int>();
        public readonly NotifyChanged<bool> SaveOnFocus = new NotifyChanged<bool>();
        public readonly NotifyChanged<bool> EnableHistory = new NotifyChanged<bool>();
        public readonly NotifiableDictionary<string, Shortcut> Hotkeys = new NotifiableDictionary<string, Shortcut>();

        private static readonly IReadOnlyDictionary<string, Shortcut> DefaultShortcuts =
            new Dictionary<string, Shortcut>
            {
                {"save", Shortcut.CtrlS},
                {"save_all", Shortcut.CtrlShiftS},
                {"open", Shortcut.CtrlO},
                {"new", Shortcut.CtrlN},
                {"exit", Shortcut.CtrlQ},
                {"undo", Shortcut.CtrlZ},
                {"redo", Shortcut.CtrlShiftZ},
                {"format", Shortcut.CtrlL},
                {"build", Shortcut.CtrlF9},
                {"run", Shortcut.CtrlShiftF9},
                {"settings", Shortcut.CtrlShiftP},
                {"bold", Shortcut.CtrlB},
                {"cursive", Shortcut.CtrlI},
                {"underline", Shortcut.CtrlU},
                {"strikethrough", Shortcut.CtrlT},
            };
        
        // For xml serialization. Do not forget to set _root after.
        private Settings()
        {
        }

        public Settings(MainForm root): this()
        {
            _root = root;
        }

        public Color GetColor(string name)
        {
            if (Colors.TryGetValue(name, out var value))
            {
                return value;
            }

            // TODO: Default theme
            return name switch
            {
                _ => Color.Black
            };
        }

        public void BindShortcut(string name, MenuItem item)
        {
            item.ShowShortcut = true;
            var fallback = DefaultShortcuts.TryGetValue(name, out var shortcut) ? shortcut : Shortcut.None;
            var hotkey = Hotkeys.GetOrDefault(name, fallback);

            item.Shortcut = hotkey.Value;
            var weak = new WeakReference<MenuItem>(item);
            hotkey.Changed += (oldValue, newValue) =>
            {
                if (weak.TryGetTarget(out var target))
                {
                    target.Shortcut = newValue;
                }
            };
        }
    }
}