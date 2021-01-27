using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Editka.Files;

namespace Editka
{
    public struct OpenedFileInfo
    {
        public string Path;
        public bool TabOpened;

        public OpenedFileInfo(string path, bool tabOpened)
        {
            Path = path;
            TabOpened = tabOpened;
        }
    }

    public class State
    {
        public Settings Settings { get; set; }

        public List<OpenedFileInfo> Files { get; set; }

        public State(Settings settings, List<OpenedFileInfo> files)
        {
            Settings = settings;
            Files = files;
        }

        // For xml serialization.
        private State()
        {
        }

        public void Serialize(MainForm root)
        {
            Files = root.FileList
                .Where(file => file.Path != null)
                .Select(file => new OpenedFileInfo(file.Path!, file.Opened != null))
                .ToList();

            var serializer = new XmlSerializer(typeof(State));
            using StringWriter textWriter = new StringWriter();
            serializer.Serialize(textWriter, this);
            var s = textWriter.ToString();
        }

        public void FillFileList(MainForm root)
        {
            foreach (var info in Files)
            {
                var file = OpenedFile.Open(info.Path);
                if (file != null)
                {
                    root.FileList.TreeView.Nodes.Add(file);
                    if (info.TabOpened)
                    {
                        root.OpenedTabs.Open(file);
                    }
                }
            }
        }
    }
}