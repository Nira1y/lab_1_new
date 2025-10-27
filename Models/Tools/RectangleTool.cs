using lab_2_graphic_editor.Models.Shapes;
using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Tools;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace lab_2_graphic_editor.Models.Tools
{
    public class RectangleTool : ShapeTool
    {
        public RectangleTool()
        {
            Name = "Прямоугольник";
            shapeModel = new RectangleShape();
        }
    }
}
