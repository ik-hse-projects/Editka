using System.Windows.Forms;
using System.Xml.Serialization;

namespace Editka.Files
{
    public abstract class BaseNode: TreeNode
    {
        private static int counter;

        public readonly int Id;
        
        private string? _path;
        
        public string? Path
        {
            get => _path;
            protected set
            {
                _path = value;
                Filename.Update();
            }
        }

        [XmlIgnore] public readonly Computed<string> Filename;

        public abstract bool IsOpened { get; }

        protected BaseNode()
        {
            Id = counter++;
            Filename = new Computed<string>(() => Path ?? "(untitled)");
            Filename.Changed += (oldValue, newValue) => Text = newValue;
            Text = Filename.Value;
        }
    }
}