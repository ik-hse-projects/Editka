using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Editka.Files;

namespace Editka
{
    /// <summary>
    /// Этот класс сохраняется на диск и хранит всё необходимое для поноценого перезапуска.
    /// </summary>
    public class State
    {
        public State()
        {
            Settings = new Settings();
        }

        /// <summary>
        /// Настройки программы.
        /// </summary>
        public Settings Settings { get; set; }

        /// <summary>
        /// Список путей к открытым файлам.
        /// </summary>
        private List<string>? Files { get; set; }

        /// <summary>
        /// Список путей к файлам, которые открыты как вкладки.
        /// </summary>
        private List<string>? OpenedTabs { get; set; }

        /// <summary>
        /// Сохраняет состояние на диск.
        /// </summary>
        /// <returns>Путь к созданному файлу.</returns>
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
                $"{now.Minute:D2}_{now.Second:D2}.{now.Millisecond:D3}");
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

        /// <summary>
        /// Загружает состояние с диска и возвращает его.
        /// </summary>
        /// <param name="path">Путь к файлу сохранения</param>
        private static State? Deserialize(string path)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(State));
                using var file = File.OpenRead(path);
                var result = (State) serializer.Deserialize(file);
                result.Settings ??= new Settings();
                return result;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Загружает и возвращает последнее сохранённое состояние.
        /// </summary>
        /// <returns></returns>
        public static State? DeserializeLatest()
        {
            try
            {
                var root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "state");
                for (var i = 0; i < 4; i++)
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

        /// <summary>
        /// Открывает все файлы и вкладки, загружает их в интерфейс.
        /// </summary>
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

        /// <summary>
        /// Создаёт копию состояния, но с обнулёнными файлами. Сохраняются только настройки.
        /// </summary>
        public State CloneSettings()
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