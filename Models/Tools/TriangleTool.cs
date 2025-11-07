using System.Windows;
using System.Windows.Controls;
using lab_2_graphic_editor.Models.Shapes;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Models.Tools
{
    public class TriangleTool : ShapeTool
    {
        public TriangleTool(ColorService colorService, CommandService commandService, bool filled = false) : base(colorService, commandService)
        {
            Name = filled ? "Треугольник (с заливкой)" : "Треугольник";
            shapeModel = new TriangleShape(colorService, filled);
        }
    }
}