using System.Windows;
using System.Windows.Controls;
using lab_2_graphic_editor.Models.Shapes;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Models.Tools
{
    public class EllipseTool : ShapeTool
    {
        public EllipseTool(ColorService colorService, CommandService commandService, bool filled = false) : base(colorService, commandService)
        {
            Name = filled ? "Эллипс (с заливкой)" : "Эллипс";
            shapeModel = new EllipseShape(colorService, filled);
        }
    }
}