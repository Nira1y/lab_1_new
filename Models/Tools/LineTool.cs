using System.Windows;
using System.Windows.Controls;
using lab_2_graphic_editor.Models.Shapes;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Models.Tools
{
    public class LineTool : ShapeTool
    {
        public LineTool(ColorService colorService, CommandService commandService) : base(colorService, commandService)
        {
            Name = "Линия";
            shapeModel = new LineShape(colorService);
        }
    }
}