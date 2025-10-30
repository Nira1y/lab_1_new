using System.Windows;
using System.Windows.Controls;
using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Models.Shapes;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Tools
{
    public class EllipseTool : ShapeTool
    {
        public EllipseTool(ColorService colorService, bool filled = false) : base(colorService)
        {
            Name = filled ? "Прямоугольник (с заливкой)" : "Прямоугольник";
            shapeModel = new EllipseShape(colorService, filled);
        }
    }
}