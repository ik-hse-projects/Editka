using System;
using System.Windows.Forms;

namespace Editka
{
    public static class MenuCreator
    {
        public static MainMenu MainMenu(MainForm root)
        {
            var menu = new MainMenu();

            {
                var file = new MenuItem("Файл");
                root.Settings.BindShortcut("save", file.MenuItems.Add("Сохранить", root.Actions.Save));
                root.Settings.BindShortcut("save_all", file.MenuItems.Add("Сохранить все", root.Actions.SaveAll));
                root.Settings.BindShortcut("open", file.MenuItems.Add("Открыть", root.Actions.Open));
                root.Settings.BindShortcut("new", file.MenuItems.Add("Создать", root.Actions.New));
                root.Settings.BindShortcut("exit", file.MenuItems.Add("Выйти", root.Actions.Exit));
                menu.MenuItems.Add(file);
            }

            {
                var edit = new MenuItem("Правка");
                root.Settings.BindShortcut("undo", edit.MenuItems.Add("Отмена", root.Actions.Undo));
                root.Settings.BindShortcut("redo", edit.MenuItems.Add("Повтор", root.Actions.Redo));
                menu.MenuItems.Add(edit);
            }

            // TODO: Disable when non-rtf file opened
            {
                var format = new MenuItem("Формат");
                AddFormatButtons(format.MenuItems, root);
                menu.MenuItems.Add(format);
            }

            // TODO: Disable when non-compilable file opened
            {
                var csharp = new MenuItem("C#");
                root.Settings.BindShortcut("format", csharp.MenuItems.Add("Отформатировать", root.Actions.Format));
                root.Settings.BindShortcut("build", csharp.MenuItems.Add("Скомпилировать", root.Actions.Build));
                root.Settings.BindShortcut("run", csharp.MenuItems.Add("Запустить", root.Actions.Run));
                menu.MenuItems.Add(csharp);
            }

            {
                root.Settings.BindShortcut("settings", menu.MenuItems.Add("Настройки", root.Actions.Settings));
            }

            return menu;
        }

        public static ContextMenu ContextMenu(MainForm root)
        {
            var menu = new ContextMenu();

            menu.MenuItems.Add("Выбрать всё", root.Actions.SelectAll);
            menu.MenuItems.Add("Вырезать", root.Actions.Cut);
            menu.MenuItems.Add("Копировать", root.Actions.Copy);
            menu.MenuItems.Add("Вставить", root.Actions.Paste);

            AddFormatButtons(menu.MenuItems, root);

            return menu;
        }

        private static MenuItem[]? _formatButtons;

        private static void AddFormatButtons(Menu.MenuItemCollection items, MainForm root)
        {
            if (_formatButtons == null)
            {
                _formatButtons = new[]
                {
                    new MenuItem("Жирный", root.Actions.Bold),
                    new MenuItem("Курсив", root.Actions.Cursive),
                    new MenuItem("Подчеркнутый", root.Actions.Underline),
                    new MenuItem("Зачёркнтуый", root.Actions.Strikethrough),
                };

                root.Settings.BindShortcut("bold", _formatButtons[0]);
                root.Settings.BindShortcut("cursive", _formatButtons[1]);
                root.Settings.BindShortcut("underline", _formatButtons[2]);
                root.Settings.BindShortcut("strikethrough", _formatButtons[3]);
            }

            // TODO: Disable when non-rtf file opened
            items.AddRange(_formatButtons);
        }
    }
}