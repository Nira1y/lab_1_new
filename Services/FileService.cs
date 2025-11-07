using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json;
using lab_2_graphic_editor.Models;

namespace lab_2_graphic_editor.Services
{
    public class FileService
    {
        public void SaveProject(Canvas canvas, string filePath)
        {
            Color backgroundColor = Colors.White;
            if (canvas.Background is SolidColorBrush solidBrush)
            {
                backgroundColor = solidBrush.Color;
            }

            var projectData = new ProjectData
            {
                Canvas = new CanvasData
                {
                    Width = canvas.ActualWidth > 0 ? canvas.ActualWidth : canvas.Width,
                    Height = canvas.ActualHeight > 0 ? canvas.ActualHeight : canvas.Height,
                    Background = new ColorData(backgroundColor)
                },
                Shapes = new List<ShapeData>(),
                Texts = new List<TextData>() 
            };

            foreach (var child in canvas.Children)
            {
                if (child is Shape shape)
                {
                    var shapeData = ConvertShapeToData(shape);
                    if (shapeData != null)
                    {
                        projectData.Shapes.Add(shapeData);
                    }
                }

                else if (child is TextBox textBox)
                {
                    var textData = ConvertTextBoxToData(textBox);
                    if (textData != null)
                    {
                        projectData.Texts.Add(textData);
                    }
                }
            }

            string json = JsonConvert.SerializeObject(projectData, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void LoadProject(Canvas canvas, string filePath)
        {
            if (!File.Exists(filePath)) return;
            string json = File.ReadAllText(filePath);
            var projectData = JsonConvert.DeserializeObject<ProjectData>(json);

            if (projectData == null)
            {
                MessageBox.Show("Неверный формат файла проекта", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            canvas.Children.Clear();

            if (projectData.Canvas != null)
            {
                canvas.Width = projectData.Canvas.Width > 0 ? projectData.Canvas.Width : 800;
                canvas.Height = projectData.Canvas.Height > 0 ? projectData.Canvas.Height : 600;

                var background = projectData.Canvas.Background != null ? projectData.Canvas.Background.ToColor() : Colors.White;
                canvas.Background = new SolidColorBrush(background);
            }
            else
            {
                canvas.Width = 800;
                canvas.Height = 600;
                canvas.Background = new SolidColorBrush(Colors.White);
            }

            if (projectData.Shapes != null)
            {
                foreach (var shapeData in projectData.Shapes)
                {
                    var shape = ConvertDataToShape(shapeData);
                    if (shape != null)
                    {
                        canvas.Children.Add(shape);
                    }
                }
            }

            if (projectData.Texts != null)
            {
                foreach (var textData in projectData.Texts)
                {
                    var textBox = ConvertDataToTextBox(textData);
                    if (textBox != null)
                    {
                        canvas.Children.Add(textBox);
                    }
                }
            }
        }

        public void ExportToImage(Canvas canvas, string filePath, string format = "png")
        {
            if (canvas.ActualWidth == 0 || canvas.ActualHeight == 0)
            {
                canvas.Width = 800;
                canvas.Height = 600;
            }

            var renderBitmap = new RenderTargetBitmap(
                (int)canvas.ActualWidth,
                (int)canvas.ActualHeight,
                96d, 96d, PixelFormats.Pbgra32);

            renderBitmap.Render(canvas);

            BitmapEncoder encoder = format.ToLower() switch
            {
                "jpg" or "jpeg" => new JpegBitmapEncoder(),
                "png" => new PngBitmapEncoder(),
                _ => new PngBitmapEncoder()
            };

            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        private TextData ConvertTextBoxToData(TextBox textBox)
        {
            if (textBox == null) return null;

            return new TextData
            {
                Text = textBox.Text,
                X = Canvas.GetLeft(textBox),
                Y = Canvas.GetTop(textBox),
                Color = textBox.Foreground is SolidColorBrush foregroundBrush
                    ? new ColorData(foregroundBrush.Color)
                    : new ColorData(Colors.Black),
                FontFamily = textBox.FontFamily?.Source ?? "Arial",
                FontSize = textBox.FontSize,
                FontWeight = new FontWeightData(textBox.FontWeight),
                FontStyle = new FontStyleData(textBox.FontStyle)
            };
        }

        private TextBox ConvertDataToTextBox(TextData textData)
        {
            if (textData == null) return null;

            var textBox = new TextBox
            {
                Text = textData.Text,
                Foreground = new SolidColorBrush(textData.Color.ToColor()),
                FontFamily = new FontFamily(textData.FontFamily),
                FontSize = textData.FontSize,
                FontWeight = textData.FontWeight.ToFontWeight(),
                FontStyle = textData.FontStyle.ToFontStyle(),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(2),
                MinWidth = 50,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                IsHitTestVisible = false, 
                Focusable = false
            };

            Canvas.SetLeft(textBox, textData.X);
            Canvas.SetTop(textBox, textData.Y);

            return textBox;
        }

        private ShapeData ConvertShapeToData(Shape shape)
        {
            if (shape == null) return null;

            var shapeData = new ShapeData
            {
                StrokeThickness = shape.StrokeThickness,
                Points = new List<PointData>()
            };

            if (shape.Stroke is SolidColorBrush strokeBrush)
            {
                shapeData.StrokeColor = new ColorData(strokeBrush.Color);
            }
            else
            {
                shapeData.StrokeColor = new ColorData(Colors.Black);
            }

            switch (shape)
            {
                case Line line:
                    shapeData.Type = "Line";
                    shapeData.Points.Add(new PointData(line.X1, line.Y1));
                    shapeData.Points.Add(new PointData(line.X2, line.Y2));
                    break;

                case Rectangle rect:
                    shapeData.Type = "Rectangle";
                    shapeData.IsFilled = rect.Fill != null &&
                                        rect.Fill != Brushes.Transparent &&
                                        !rect.Fill.Equals(Brushes.Transparent);

                    if (shapeData.IsFilled && rect.Fill is SolidColorBrush fillBrushRect)
                    {
                        shapeData.FillColor = new ColorData(fillBrushRect.Color);
                    }
                    else
                    {
                        shapeData.FillColor = new ColorData(Colors.Transparent);
                    }

                    double rectLeft = Canvas.GetLeft(rect);
                    double rectTop = Canvas.GetTop(rect);
                    shapeData.Points.Add(new PointData(rectLeft, rectTop));
                    shapeData.Points.Add(new PointData(rectLeft + rect.Width, rectTop + rect.Height));
                    break;

                case Ellipse ellipse:
                    shapeData.Type = "Ellipse";
                    shapeData.IsFilled = ellipse.Fill != null &&
                                        ellipse.Fill != Brushes.Transparent &&
                                        !ellipse.Fill.Equals(Brushes.Transparent);

                    if (shapeData.IsFilled && ellipse.Fill is SolidColorBrush fillBrushEllipse)
                    {
                        shapeData.FillColor = new ColorData(fillBrushEllipse.Color);
                    }
                    else
                    {
                        shapeData.FillColor = new ColorData(Colors.Transparent);
                    }

                    double ellipseLeft = Canvas.GetLeft(ellipse);
                    double ellipseTop = Canvas.GetTop(ellipse);
                    shapeData.Points.Add(new PointData(ellipseLeft, ellipseTop));
                    shapeData.Points.Add(new PointData(ellipseLeft + ellipse.Width, ellipseTop + ellipse.Height));
                    break;

                case Polygon polygon:
                    shapeData.Type = "Triangle";
                    shapeData.IsFilled = polygon.Fill != null &&
                                        polygon.Fill != Brushes.Transparent &&
                                        !polygon.Fill.Equals(Brushes.Transparent);

                    if (shapeData.IsFilled && polygon.Fill is SolidColorBrush fillBrushPolygon)
                    {
                        shapeData.FillColor = new ColorData(fillBrushPolygon.Color);
                    }
                    else
                    {
                        shapeData.FillColor = new ColorData(Colors.Transparent);
                    }

                    foreach (var point in polygon.Points)
                    {
                        shapeData.Points.Add(new PointData(point.X, point.Y));
                    }
                    break;

                case Polyline polyline:
                    shapeData.Type = "Curve";
                    shapeData.IsCurve = true;

                    foreach (var point in polyline.Points)
                    {
                        shapeData.Points.Add(new PointData(point.X, point.Y));
                    }
                    break;

                default:
                    return null;
            }

            return shapeData;
        }

        private Shape ConvertDataToShape(ShapeData shapeData)
        {
            if (shapeData == null || shapeData.Points == null) return null;

            Shape shape = shapeData.Type switch
            {
                "Line" => new Line(),
                "Rectangle" => new Rectangle(),
                "Ellipse" => new Ellipse(),
                "Triangle" => new Polygon(),
                "Curve" => new Polyline(),
                _ => null
            };

            if (shape == null) return null;

            Color strokeColor = shapeData.StrokeColor?.ToColor() ?? Colors.Black;
            shape.Stroke = new SolidColorBrush(strokeColor);
            shape.StrokeThickness = shapeData.StrokeThickness;

            switch (shape)
            {
                case Line line when shapeData.Points.Count >= 2:
                    line.X1 = shapeData.Points[0].X;
                    line.Y1 = shapeData.Points[0].Y;
                    line.X2 = shapeData.Points[1].X;
                    line.Y2 = shapeData.Points[1].Y;
                    break;

                case Rectangle rect when shapeData.Points.Count >= 2:
                    double rectLeft = shapeData.Points[0].X;
                    double rectTop = shapeData.Points[0].Y;
                    double rectRight = shapeData.Points[1].X;
                    double rectBottom = shapeData.Points[1].Y;

                    Canvas.SetLeft(rect, Math.Min(rectLeft, rectRight));
                    Canvas.SetTop(rect, Math.Min(rectTop, rectBottom));
                    rect.Width = Math.Abs(rectRight - rectLeft);
                    rect.Height = Math.Abs(rectBottom - rectTop);

                    if (shapeData.IsFilled && shapeData.FillColor != null)
                    {
                        Color fillColor = shapeData.FillColor.ToColor();
                        if (fillColor != Colors.Transparent)
                        {
                            rect.Fill = new SolidColorBrush(fillColor);
                        }
                        else
                        {
                            rect.Fill = Brushes.Transparent;
                        }
                    }
                    else
                    {
                        rect.Fill = Brushes.Transparent;
                    }
                    break;

                case Ellipse ellipse when shapeData.Points.Count >= 2:
                    double ellipseLeft = shapeData.Points[0].X;
                    double ellipseTop = shapeData.Points[0].Y;
                    double ellipseRight = shapeData.Points[1].X;
                    double ellipseBottom = shapeData.Points[1].Y;

                    Canvas.SetLeft(ellipse, Math.Min(ellipseLeft, ellipseRight));
                    Canvas.SetTop(ellipse, Math.Min(ellipseTop, ellipseBottom));
                    ellipse.Width = Math.Abs(ellipseRight - ellipseLeft);
                    ellipse.Height = Math.Abs(ellipseBottom - ellipseTop);

                    if (shapeData.IsFilled && shapeData.FillColor != null)
                    {
                        Color fillColor = shapeData.FillColor.ToColor();
                        if (fillColor != Colors.Transparent)
                        {
                            ellipse.Fill = new SolidColorBrush(fillColor);
                        }
                        else
                        {
                            ellipse.Fill = Brushes.Transparent;
                        }
                    }
                    else
                    {
                        ellipse.Fill = Brushes.Transparent;
                    }
                    break;

                case Polygon polygon when shapeData.Points.Count > 0:
                    var points = new PointCollection();
                    foreach (var pointData in shapeData.Points)
                    {
                        points.Add(new System.Windows.Point(pointData.X, pointData.Y));
                    }
                    polygon.Points = points;

                    if (shapeData.IsFilled && shapeData.FillColor != null)
                    {
                        Color fillColor = shapeData.FillColor.ToColor();
                        if (fillColor != Colors.Transparent)
                        {
                            polygon.Fill = new SolidColorBrush(fillColor);
                        }
                        else
                        {
                            polygon.Fill = Brushes.Transparent;
                        }
                    }
                    else
                    {
                        polygon.Fill = Brushes.Transparent;
                    }
                    break;
                case Polyline polyline when shapeData.Points.Count > 0:
                    var _points = new PointCollection();
                    foreach (var pointData in shapeData.Points)
                    {
                        _points.Add(new System.Windows.Point(pointData.X, pointData.Y));
                    }
                    polyline.Points = _points;
                    polyline.StrokeLineJoin = PenLineJoin.Round;
                    break;

                default:
                    return null;
            }
            return shape;
        }
    }
}