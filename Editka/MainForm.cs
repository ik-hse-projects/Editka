using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;

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

        internal MainForm()
        {
            // Чтобы хоткеи работали:
            KeyPreview = true;

            State = new State(
                new Settings(this),
                new List<OpenedFileInfo>()
            );

            FileList = new FileList(this);
            Actions = new Actions(this);
            Menu = MenuCreator.MainMenu(this);
            Autosave.Init(this);
            Theme = new ColorScheme(this);
            OpenedTabs = new Tabs(this)
            {
                Dock = DockStyle.Fill
            };
            Notes = new Notes
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
            Controls.Add(container1);
        }
    }
}