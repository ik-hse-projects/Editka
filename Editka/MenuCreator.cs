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
                root.Settings.BindShortcut("save", file.MenuItems.Add("Сохранить", root.Actions.TODO));
                root.Settings.BindShortcut("save_all", file.MenuItems.Add("Сохранить все", root.Actions.TODO));
                root.Settings.BindShortcut("open", file.MenuItems.Add("Открыть", root.Actions.TODO));
                root.Settings.BindShortcut("new", file.MenuItems.Add("Создать", root.Actions.TODO));
                root.Settings.BindShortcut("exit", file.MenuItems.Add("Выйти", root.Actions.Exit));
                menu.MenuItems.Add(file);
            }

            {
                var edit = new MenuItem("Правка");
                root.Settings.BindShortcut("undo", edit.MenuItems.Add("Отмена", root.Actions.TODO));
                root.Settings.BindShortcut("redo", edit.MenuItems.Add("Повтор", root.Actions.TODO));
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
                root.Settings.BindShortcut("build", csharp.MenuItems.Add("Скомпилировать", root.Actions.TODO));
                root.Settings.BindShortcut("run", csharp.MenuItems.Add("Запустить", root.Actions.TODO));
                menu.MenuItems.Add(csharp);
            }

            {
                root.Settings.BindShortcut("settings", menu.MenuItems.Add("Настройки", root.Actions.TODO));
            }

            return menu;
        }

        public static ContextMenu ContextMenu(MainForm root)
        {
            var menu = new ContextMenu();

            menu.MenuItems.Add("Выбрать всё", root.Actions.TODO);
            menu.MenuItems.Add("Вырезать", root.Actions.TODO);
            menu.MenuItems.Add("Копировать", root.Actions.TODO);
            menu.MenuItems.Add("Вставить", root.Actions.TODO);

            AddFormatButtons(menu.MenuItems, root);

            return menu;
        }

        private static MenuItem[]? FormatButtons;

        private static void AddFormatButtons(Menu.MenuItemCollection items, MainForm root)
        {
            if (FormatButtons == null)
            {
                FormatButtons = new[]
                {
                    new MenuItem("Жирный", root.Actions.TODO),
                    new MenuItem("Курсив", root.Actions.TODO),
                    new MenuItem("Подчеркнутый", root.Actions.TODO),
                    new MenuItem("Зачёркнтуый", root.Actions.TODO),
                };

                root.Settings.BindShortcut("bold", FormatButtons[0]);
                root.Settings.BindShortcut("cursive", FormatButtons[1]);
                root.Settings.BindShortcut("underline", FormatButtons[2]);
                root.Settings.BindShortcut("strikethrough", FormatButtons[3]);
            }

            // TODO: Disable when non-rtf file opened
            items.AddRange(FormatButtons);
        }
    }
}