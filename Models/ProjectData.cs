using System.Collections.Generic;
using System.Windows.Media;

namespace lab_2_graphic_editor.Models
{
    public class ProjectData
    {
        public List<ShapeData> Shapes { get; set; } = new List<ShapeData>();
        public CanvasData Canvas { get; set; } = new CanvasData(); 
    }

    public class ShapeData
    {
        public string Type { get; set; } = "";
        public List<PointData> Points { get; set; } = new List<PointData>();
        public ColorData StrokeColor { get; set; } = new ColorData(Colors.Black);
        public ColorData FillColor { get; set; } = new ColorData(Colors.Transparent);
        public double StrokeThickness { get; set; } = 2;
        public bool IsFilled { get; set; }
        public List<PointData> BrushPoints { get; set; } = new List<PointData>();
        public double BrushSize { get; set; } = 3;
    }

    public class PointData
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointData() { }
        public PointData(double x, double y) { X = x; Y = y; }
    }

    public class ColorData
    {
        public byte A { get; set; } = 255;
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public ColorData() { }
        public ColorData(Color color)
        {
            A = color.A;
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public Color ToColor()
        {
            return Color.FromArgb(A, R, G, B);
        }
    }

    public class CanvasData
    {
        public double Width { get; set; } = 800;
        public double Height { get; set; } = 600;
        public ColorData Background { get; set; } = new ColorData(Colors.White);
    }
}