using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Editka
{
    public class Settings
    {
        private readonly MainForm _root;
        private readonly ColorConverter _colorConverter = new ColorConverter();
        public readonly NotifiableDictionary<string, Color> Colors = new NotifiableDictionary<string, Color>();

        public readonly NotifyChanged<int> AutosaveSeconds = new NotifyChanged<int>();
        public readonly NotifyChanged<bool> SaveOnFocus = new NotifyChanged<bool>();
        public readonly NotifyChanged<bool> EnableHistory = new NotifyChanged<bool>();
        
        public Settings(MainForm root)
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

        private readonly NotifiableDictionary<string, string> _hotkeys = new NotifiableDictionary<string, string>();
        private readonly NotifiableDictionary<string, MenuItem> _binded = new NotifiableDictionary<string, MenuItem>();
        
        public static IReadOnlyDictionary<string, Shortcut> DefaultShortcuts = new Dictionary<string, Shortcut>
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

        public Shortcut GetShortcut(string name)
        {
            if (_hotkeys.TryGetValue(name, out var value)
                && Enum.TryParse<Shortcut>(value, true, out var shortcut))
            {
                return shortcut;
            }

            if (DefaultShortcuts.TryGetValue(name, out var fallback))
            {
                return fallback;
            }

            return Shortcut.None;
        }

        public void BindShortcut(string name, MenuItem item)
        {
            item.ShowShortcut = true;
            item.Shortcut = GetShortcut(name);
            _binded[name] = item;
        }

        private void UpdateShortcuts()
        {
            foreach (var kv in _binded)
            {
                kv.Value.Shortcut = GetShortcut(kv.Key);
            }
        }
    }
}