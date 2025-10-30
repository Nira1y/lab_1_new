using System.Windows;
using System.Windows.Controls;
using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Models.Shapes;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Tools
{
    public class LineTool : ShapeTool
    {
        public LineTool(ColorService colorService) : base(colorService)
        {
            Name = "Линия";
            IconPath = "/Resources/line_icon.png";
            shapeModel = new LineShape(colorService);
        }
    }
}