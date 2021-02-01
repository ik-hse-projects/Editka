using System.Windows.Forms;

namespace Editka
{
    /// <summary>
    /// Класс, который занимается созданием менюшек на все случаи жизни.
    /// </summary>
    public static class MenuCreator
    {
        /// <summary>
        /// Те пункты меню, которые отвечают за форматирование, и на которые навешаны хоткеи.
        /// </summary>
        private static MenuItem[]? _formatButtons;

        /// <summary>
        /// Создаёт главное меню всея программмы (то, что сверху).
        /// </summary>
        public static MainMenu MainMenu(MainForm root)
        {
            var menu = new MainMenu();

            {
                var file = new MenuItem("Файл");
                root.Settings.BindShortcut("save", file.MenuItems.Add("Сохранить", root.Actions.Save));
                root.Settings.BindShortcut("save_all", file.MenuItems.Add("Сохранить все", root.Actions.SaveAll));
                root.Settings.BindShortcut("open", file.MenuItems.Add("Открыть", root.Actions.Open));
                root.Settings.BindShortcut("open_dir", file.MenuItems.Add("Открыть папку", root.Actions.OpenDirectory));
                root.Settings.BindShortcut("open_in_new",
                    file.MenuItems.Add("Открыть в новом окне", root.Actions.OpenInNewWindow));
                root.Settings.BindShortcut("new", file.MenuItems.Add("Создать", root.Actions.New));
                root.Settings.BindShortcut("create_in_new",
                    file.MenuItems.Add("Создать в новом окне", root.Actions.CreateInNewWindow));
                root.Settings.BindShortcut("exit", file.MenuItems.Add("Выйти", root.Actions.Exit));
                root.Settings.BindShortcut("new_window", file.MenuItems.Add("Новое окно", root.Actions.NewWindow));
                menu.MenuItems.Add(file);
            }

            {
                var edit = new MenuItem("Правка");
                root.Settings.BindShortcut("undo", edit.MenuItems.Add("Отмена", root.Actions.Undo));
                root.Settings.BindShortcut("redo", edit.MenuItems.Add("Повтор", root.Actions.Redo));
                menu.MenuItems.Add(edit);
            }

            {
                var format = new MenuItem("Формат");
                AddFormatButtons(format.MenuItems, root);
                menu.MenuItems.Add(format);
            }

            {
                var csharp = new MenuItem("C#");
                root.Settings.BindShortcut("build", csharp.MenuItems.Add("Скомпилировать", root.Actions.Build));
                menu.MenuItems.Add(csharp);
            }

            {
                root.Settings.BindShortcut("settings", menu.MenuItems.Add("Настройки", root.Actions.Settings));
            }

            return menu;
        }

        /// <summary>
        /// Создаёт контекстное меню для текстовых файлов.
        /// </summary>
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

        /// <summary>
        /// Добавляет кнопки форматирования в указанное меню.
        /// </summary>
        /// <param name="items">Куда нужно добавить кнопки</param>
        private static void AddFormatButtons(Menu.MenuItemCollection items, MainForm root)
        {
            var formatButtons = new[]
            {
                new MenuItem("Жирный", root.Actions.Bold),
                new MenuItem("Курсив", root.Actions.Cursive),
                new MenuItem("Подчеркнутый", root.Actions.Underline),
                new MenuItem("Зачёркнтуый", root.Actions.Strikethrough)
            };

            if (_formatButtons == null)
            {
                _formatButtons = formatButtons;

                root.Settings.BindShortcut("bold", _formatButtons[0]);
                root.Settings.BindShortcut("cursive", _formatButtons[1]);
                root.Settings.BindShortcut("underline", _formatButtons[2]);
                root.Settings.BindShortcut("strikethrough", _formatButtons[3]);
            }

            items.AddRange(formatButtons);
        }
    }
}