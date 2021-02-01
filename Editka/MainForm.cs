using System.Windows.Forms;
using Editka.Compat;

namespace Editka
{
    /// <summary>
    /// Основная формочка. Содержит всё, необходимое для работы программы.
    /// </summary>
    public class MainForm : Form
    {
        /// <summary>
        /// То состояние, которое сохраняется при перезапуске.
        /// </summary>
        public State State;

        internal MainForm(State state)
        {
            // Чтобы хоткеи работали:
            KeyPreview = true;

            State = state;

            FileList = new FileList(this);
            Actions = new Actions(this);
            Autosave.Init(this);
            var theme = new ColorScheme(this);
            OpenedTabs = new Tabs(this)
            {
                Dock = DockStyle.Fill
            };
            Notes = new Notes(this)
            {
                Dock = DockStyle.Fill
            };

            theme.ApplyTo(this);
            State.FillFileList(this);

            Closing += Actions.Exit;

            var container1 = new SplitContainer
            {
                Orientation = Orientation.Vertical,
                BorderStyle = BorderStyle.Fixed3D,
                Dock = DockStyle.Fill
            };
            var container2 = new SplitContainer
            {
                Orientation = Orientation.Vertical,
                BorderStyle = BorderStyle.Fixed3D,
                Dock = DockStyle.Fill
            };
            container2.Panel1.Controls.Add(OpenedTabs);
            container2.Panel2.Controls.Add(Notes);
            container1.Panel1.Controls.Add(FileList.TreeView);
            container1.Panel2.Controls.Add(container2);

            this.SetContent(MenuCreator.MainMenu(this), container1);
        }

        /// <summary>
        /// Всевозможные настраиваемые параметры.
        /// </summary>
        public Settings Settings => State.Settings;
        
        /// <summary>
        /// Всевозможные действия, которые может пожелать сделать пользователь.
        /// </summary>
        public Actions Actions { get; }

        /// <summary>
        /// Вкладки, открытые на данный момент.
        /// </summary>
        public Tabs OpenedTabs { get; }
        
        /// <summary>
        /// Файл, который на данный момент сфокусирован.
        /// </summary>
        public FileView? CurrentFile => OpenedTabs.SelectedTab;

        /// <summary>
        /// Список откртых файлов.
        /// </summary>
        public FileList FileList { get; }

        /// <summary>
        /// Панелька для важной информации.
        /// </summary>
        public Notes Notes { get; }
    }
}