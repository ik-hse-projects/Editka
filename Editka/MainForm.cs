﻿using System.Windows.Forms;

namespace Editka
{
    public class MainForm : Form
    {
        public Settings Settings { get; }
        public Actions Actions { get; }

        public CurrentFile CurrentFile { get; }

        public FileList FileList { get; }

        public Notes Notes { get; }

        public static void Main()
        {
            Application.Run(new MainForm());
        }

        private MainForm()
        {
            // Чтобы хоткеи работали:
            KeyPreview = true;

            Settings = new Settings(this);
            Actions = new Actions(this);
            Menu = MenuCreator.MainMenu(this);

            FileList = new FileList
            {
                Dock = DockStyle.Fill
            };
            CurrentFile = new CurrentFile
            {
                Dock = DockStyle.Fill
            };
            Notes = new Notes
            {
                Dock = DockStyle.Fill
            };

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
            container2.Panel1.Controls.Add(CurrentFile);
            container2.Panel2.Controls.Add(Notes);
            container1.Panel1.Controls.Add(FileList);
            container1.Panel2.Controls.Add(container2);
            Controls.Add(container1);
        }
    }
}