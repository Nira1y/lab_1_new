using System.Windows;
using System.Windows.Controls;
using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Models.Shapes;
using lab_2_graphic_editor.Tools;

namespace lab_2_graphic_editor.Models.Tools
{
    public class EllipseTool : ShapeTool
    {
        public EllipseTool()
        {
            Name = "Эллипс";
            shapeModel = new EllipseShape();
        }
    }
}
