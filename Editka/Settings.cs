using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Editka
{
    /// <summary>
    /// Альтернативная реализация <seealso cref="System.Drawing.Color"/>, но которая нормально сериализуется.
    /// </summary>
    public struct SerializableColor
    {
        [XmlIgnore] public Color Color;

        public string ColorHtml
        {
            get => ColorTranslator.ToHtml(Color);
            set => Color = ColorTranslator.FromHtml(value);
        }
    }

    /// <summary>
    /// Все настройки программы.
    /// </summary>
    // Этот класс активно сериализуектся и десериализуется, поэтому нельзя делать readonly.
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    public class Settings
    {
        /// <summary>
        /// Хоткеи по-умолчанию.
        /// </summary>
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
                {"strikethrough", Shortcut.CtrlT}
            };

        /// <summary>
        /// Период автосохранения.
        /// </summary>
        public NotifyChanged<int> AutosaveSeconds = new NotifyChanged<int>();

        /// <summary>
        /// Различные цвета программы.
        /// </summary>
        public NotifiableDictionary<string, SerializableColor> Colors =
            new NotifiableDictionary<string, SerializableColor>();

        /// <summary>
        /// Путь к csc.exe
        /// </summary>
        public NotifyChanged<string?> CscPath = new NotifyChanged<string?>();

        /// <summary>
        /// Путь к dotnet.exe
        /// </summary>
        public NotifyChanged<string?> DotnetPath = new NotifyChanged<string?>();
        
        /// <summary>
        /// Настроенные хоткеи.
        /// </summary>
        public NotifiableDictionary<string, Shortcut> Hotkeys = new NotifiableDictionary<string, Shortcut>();

        /// <summary>
        /// Привязывает хоткей к данному пункту меню.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="item">Пункт меню.</param>
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