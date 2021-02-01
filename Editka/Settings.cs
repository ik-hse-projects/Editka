using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Editka.Compat;

namespace Editka
{
    public struct SerializableColor
    {
        [XmlIgnore] public Color Color;

        public string ColorHtml
        {
            get => ColorTranslator.ToHtml(Color);
            set => Color = ColorTranslator.FromHtml(value);
        }
    }

    public class Settings
    {
        public NotifiableDictionary<string, SerializableColor> Colors = new NotifiableDictionary<string, SerializableColor>();
        public NotifyChanged<int> AutosaveSeconds = new NotifyChanged<int>();
        public NotifyChanged<bool> SaveOnFocus = new NotifyChanged<bool>();
        public readonly NotifyChanged<bool> EnableHistory = new NotifyChanged<bool>();
        public NotifiableDictionary<string, Shortcut> Hotkeys = new NotifiableDictionary<string, Shortcut>();

        public NotifyChanged<string?> DotnetPath = new NotifyChanged<string?>();
        public NotifyChanged<string?> CscPath = new NotifyChanged<string?>();

        private static readonly IReadOnlyDictionary<string, Shortcut> DefaultShortcuts =
            new Dictionary<string, Shortcut>
            {
                {"save", Shortcut.CtrlS},
                {"save_all", Shortcut.CtrlShiftS},
                {"open", Shortcut.CtrlO},
                {"new", Shortcut.CtrlN},
                {"create_in_new", Shortcut.CtrlShiftN},
                {"exit", Shortcut.CtrlQ},
                {"undo", Shortcut.CtrlZ},
                {"redo", Shortcut.CtrlShiftZ},
                {"build", Shortcut.CtrlF9},
                {"settings", Shortcut.CtrlShiftP},
                {"bold", Shortcut.CtrlB},
                {"cursive", Shortcut.CtrlI},
                {"underline", Shortcut.CtrlU},
                {"strikethrough", Shortcut.CtrlT},
            };

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