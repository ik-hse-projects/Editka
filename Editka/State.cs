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
        public Settings? Settings { get; set; }

        public List<OpenedFileInfo>? Files { get; set; }

        public State()
        {
        }

        public string? Serialize(MainForm root)
        {
            Files = root.FileList
                .Where(file => file.Path != null)
                .Select(file => new OpenedFileInfo(file.Path!, file.IsOpened))
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

            foreach (var info in Files)
            {
                var file = OpenedFile.Open(info.Path);
                if (file != null)
                {
                    root.FileList.TreeView.Nodes.Add(file);
                    if (info.TabOpened && file is OpenedFile openedFile)
                    {
                        root.OpenedTabs.Open(openedFile);
                    }
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