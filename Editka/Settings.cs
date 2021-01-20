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
        private readonly Dictionary<string, string> _colors = new Dictionary<string, string>();

        public Settings(MainForm root)
        {
            _root = root;
        }

        public Color GetColor(string name)
        {
            if (_colors.TryGetValue(name, out var value))
            {
                var converted = _colorConverter.ConvertFromInvariantString(value);
                if (converted is Color casted)
                {
                    return casted;
                }
            }

            // TODO: Default theme
            return name switch
            {
                _ => Color.Black
            };
        }

        private readonly Dictionary<string, string> _hotkeys = new Dictionary<string, string>();
        private readonly Dictionary<string, MenuItem> _binded = new Dictionary<string, MenuItem>();

        public Shortcut GetShortcut(string name)
        {
            if (_hotkeys.TryGetValue(name, out var value)
                && Enum.TryParse<Shortcut>(value, true, out var shortcut))
            {
                return shortcut;
            }

            // TODO: default hotkeys
            return name switch
            {
                "bold" => Shortcut.CtrlB,
                "exit" => Shortcut.CtrlQ,
                "undo" => Shortcut.CtrlZ,
                "redo" => Shortcut.CtrlShiftZ,
                _ => Shortcut.None
            };
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