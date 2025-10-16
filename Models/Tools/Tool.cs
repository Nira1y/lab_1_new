using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace lab_2_graphic_editor.Models.Tools
{
    public abstract class Tool
    {
        public string Name { get; protected set; }
        public string IconPath { get; protected set; }

        public abstract void OnMouseDown(Point position, Canvas canvas);
        public abstract void OnMouseUp(Point position, Canvas canvas);
        public abstract void OnMouseMove(Point position, Canvas canvas);
    }
}
