using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace lab_2_graphic_editor.Services
{
    public class SelectionService
    {
        private Dictionary<UIElement, ElementProperties> _originalProperties = new Dictionary<UIElement, ElementProperties>();

        public UIElement FindElementAtPosition(Point position, Canvas canvas)
        {
            if (canvas == null) return null;

            try
            {
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, position);

                if (hitTestResult != null)
                {
                    DependencyObject current = hitTestResult.VisualHit;
                    while (current != null && current != canvas)
                    {
                        if (current is Shape shape || current is TextBox textBox)
                        {
                            return current as UIElement;
                        }
                        current = VisualTreeHelper.GetParent(current);
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public void SaveOriginalProperties(UIElement element)
        {
            if (element == null) return;

            if (!_originalProperties.ContainsKey(element))
            {
                var properties = new ElementProperties();

                if (element is Shape shape)
                {
                    properties.Stroke = shape.Stroke;
                    properties.StrokeThickness = shape.StrokeThickness;
                    properties.StrokeDashArray = shape.StrokeDashArray;
                }
                else if (element is TextBox textBox)
                {
                    properties.BorderThickness = textBox.BorderThickness;
                    properties.BorderBrush = textBox.BorderBrush;
                }

                _originalProperties[element] = properties;
            }
        }

        public void HighlightSelectedElement(UIElement element)
        {
            if (element == null) return;

            SaveOriginalProperties(element);

            if (element is Shape shape)
            {
                shape.Stroke = Brushes.Blue;
                shape.StrokeThickness = GetOriginalStrokeThickness(element) + 2;
                shape.StrokeDashArray = new DoubleCollection(new double[] { 4, 2 });
            }
            else if (element is TextBox textBox)
            {
                textBox.BorderThickness = new Thickness(2);
                textBox.BorderBrush = Brushes.Blue;
            }
        }

        public void ClearElementHighlight(UIElement element)
        {
            if (element == null || !_originalProperties.ContainsKey(element)) return;

            var properties = _originalProperties[element];

            if (element is Shape shape)
            {
                shape.Stroke = properties.Stroke;
                shape.StrokeThickness = properties.StrokeThickness;
                shape.StrokeDashArray = properties.StrokeDashArray;
            }
            else if (element is TextBox textBox)
            {
                textBox.BorderThickness = properties.BorderThickness;
                textBox.BorderBrush = properties.BorderBrush;
            }

            _originalProperties.Remove(element);
        }

        public void UpdateStrokeColor(UIElement element, Color color)
        {
            if (element == null) return;

            if (element is Shape shape)
            {
                if (!_originalProperties.ContainsKey(element))
                {
                    SaveOriginalProperties(element);
                }

                shape.Stroke = new SolidColorBrush(color);

                if (_originalProperties.ContainsKey(element))
                {
                    _originalProperties[element].Stroke = new SolidColorBrush(color);
                }
            }
        }

        public void UpdateFillColor(UIElement element, Color color)
        {
            if (element is Shape shape && shape is not Line)
            {
                shape.Fill = new SolidColorBrush(color);
            }
        }

        private double GetOriginalStrokeThickness(UIElement element)
        {
            if (_originalProperties.ContainsKey(element))
            {
                return _originalProperties[element].StrokeThickness;
            }
            return 1.0; 
        }
        private class ElementProperties
        {
            public Brush Stroke { get; set; }
            public double StrokeThickness { get; set; }
            public DoubleCollection StrokeDashArray { get; set; }
            public Thickness BorderThickness { get; set; }
            public Brush BorderBrush { get; set; }
        }

    }
}