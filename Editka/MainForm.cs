using System.Windows.Forms;

namespace Editka
{
    public class MainForm : Form
    {
        public State State;

        public Settings Settings => State.Settings;
        public Actions Actions { get; }

        public Tabs OpenedTabs { get; }
        public FileView? CurrentFile => OpenedTabs.SelectedTab;

        public FileList FileList { get; }

        public ColorScheme Theme { get; }

        public Notes Notes { get; }

        internal MainForm(State state)
        {
            // Чтобы хоткеи работали:
            KeyPreview = true;

            State = state;

            FileList = new FileList(this);
            Actions = new Actions(this);
            Autosave.Init(this);
            Theme = new ColorScheme(this);
            OpenedTabs = new Tabs(this)
            {
                Dock = DockStyle.Fill
            };
            Notes = new Notes(this)
            {
                Dock = DockStyle.Fill
            };

            Theme.ApplyTo(this);
            State.FillFileList(this);

            Closing += Actions.Exit;

            var container1 = new SplitContainer
            {
                Orientation = Orientation.Vertical,
                BorderStyle = BorderStyle.Fixed3D,
                Dock = DockStyle.Fill,
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

            MainMenuStrip = MenuCreator.MainMenu(this);
            Controls.Add(new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                Controls=
                {
                    MainMenuStrip,
                    container1
                }
            });
        }
    }
}