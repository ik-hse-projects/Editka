using System;
using System.IO;

namespace Editka
{
    /// <summary>
    /// TODO: Дока
    /// </summary>
    /// <remarks>
    /// Если забыть Dispose — будет плохо. Пользователь будет недоволен. Не надо так.
    /// </remarks>
    public class OpenedFile : IDisposable
    {
        public FileStream File { get; }
        public string Path { get; }

        /// <summary>
        /// При помощи диалога запрашивает у пользователя путь к файцлу и пытается его открыть.
        /// </summary>
        /// <param name="allowCreation">Разрешить ли создание новых файлов. Если нет, то можно только открывать существующие.</param>
        /// <returns>OpenedFile, если всё прошло удачно, иначе null.</returns>
        public static OpenedFile? AskOpen(bool allowCreation)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
        }

        public void Dispose()
        {
            File.Dispose();
        }
    }
}