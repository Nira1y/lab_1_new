using lab_2_graphic_editor.Models.Shapes;
using lab_2_graphic_editor.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab_2_graphic_editor.Models.Tools
{
    public class TriangleTool : ShapeTool
    {
        public TriangleTool() 
        {
            Name = "Треугольник";
            shapeModel = new TriangleShape();
        }
    }
}
