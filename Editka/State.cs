using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Editka.Files;

namespace Editka
{
    public class State
    {
        public Settings? Settings { get; set; }

        public List<string>? Files { get; set; }
        public List<string>? OpenedTabs { get; set; }

        public State()
        {
        }

        public string? Serialize(MainForm root)
        {
            Files = root.FileList.TopNodes
                .Select(file => file.Path!)
                .Where(path => path != null)
                .ToList();
            OpenedTabs = root.OpenedTabs.FileTabs
                .Select(tab => tab.File.Path)
                .Where(path => path != null)
                .ToList();

            var serializer = new XmlSerializer(typeof(State));

            var now = DateTime.UtcNow;
            var path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "state",
                $"{now.Year:D4}",
                $"{now.Month:D2}.{now.Day:D2}", $"{now.Hour:D2}",
                $"{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D3}");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using var file = File.OpenWrite(path);
                serializer.Serialize(file, this);
                return path;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return null;
            }
        }

        public static State? Deserialize(string path)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(State));
                using var file = File.OpenRead(path);
                return (State) serializer.Deserialize(file);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return null;
            }
        }

        public static State? DeserializeLatest()
        {
            try
            {
                var root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "state");
                for (int i = 0; i < 4; i++)
                {
                    // Выбираем наибольшее имя.
                    root = Directory.EnumerateFileSystemEntries(root)
                        .OrderByDescending(x => x)
                        .FirstOrDefault();
                    if (root == null)
                    {
                        return null;
                    }
                }

                return Deserialize(root);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return null;
            }
        }

        public void FillFileList(MainForm root)
        {
            if (Files == null)
            {
                return;
            }

            foreach (var path in Files)
            {
                var file = BaseNode.Open(path);
                if (file != null)
                {
                    root.FileList.TreeView.Nodes.Add(file);
                }
            }

            var opened = OpenedTabs != null ? OpenedTabs.ToHashSet() : new HashSet<string>();
            foreach (var node in root.FileList.WalkTree())
            {
                if (node is OpenedFile openedFile && opened.Contains(openedFile.Path))
                {
                    root.OpenedTabs.Open(openedFile);
                }
            }
        }

        public State Clone()
        {
            var serializer = new XmlSerializer(typeof(State));

            string serialized;
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, this);
                serialized = writer.ToString();
            }

            State result;
            using (var reader = new StringReader(serialized))
            {
                result = (State) serializer.Deserialize(reader);
            }

            result.Files?.Clear();
            return result;
        }
    }
}