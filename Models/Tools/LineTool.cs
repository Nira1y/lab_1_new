using System.Windows;
using System.Windows.Controls;
using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Models.Shapes;

namespace lab_2_graphic_editor.Tools
{
    public class LineTool : ShapeTool
    {
        public LineTool()
        {
            Name = "Линия";
            shapeModel = new LineShape();
        }
    }
}