using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Models.Shapes;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Tools
{
    public abstract class ShapeTool : Tool
    {
        protected Point startPoint;
        protected Shape currentShape;
        protected bool isDrawing = false;
        protected ShapeBase shapeModel;

        protected ShapeTool(ColorService colorService)
        {
            // ColorService передается в конкретные ShapeTool через конструкторы наследников
        }

        public override void OnMouseDown(Point position, Canvas canvas)
        {
            isDrawing = true;
            startPoint = position;

            currentShape = shapeModel.CreateShape(startPoint, position);
            canvas.Children.Add(currentShape);
        }

        public override void OnMouseMove(Point position, Canvas canvas)
        {
            if (isDrawing && currentShape != null)
            {
                shapeModel.UpdateShape(currentShape, startPoint, position);
            }
        }

        public override void OnMouseUp(Point position, Canvas canvas)
        {
            isDrawing = false;
            currentShape = null;
        }
    }
}