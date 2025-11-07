using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace lab_2_graphic_editor.Services
{
    public class SelectionService
    {
        private Dictionary<UIElement, ElementProperties> _originalProperties = new Dictionary<UIElement, ElementProperties>();
        private Dictionary<Canvas, Border> _canvasOverlays = new Dictionary<Canvas, Border>();
        private Canvas _mainCanvas;

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
                        if (current is Shape shape || current is TextBox textBox || current is Polyline)
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
                else if (element is Canvas canvas)
                {
  
                    properties.CanvasLeft = Canvas.GetLeft(canvas);
                    properties.CanvasTop = Canvas.GetTop(canvas);
                    properties.CanvasZIndex = Canvas.GetZIndex(canvas);
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
            else if (element is Polyline polyline)
            {
                polyline.Stroke = Brushes.Blue;
                polyline.StrokeThickness = GetOriginalStrokeThickness(element) + 2;
                polyline.StrokeDashArray = new DoubleCollection(new double[] { 4, 2 });
            }
            else if (element is TextBox textBox)
            {
                textBox.BorderThickness = new Thickness(2);
                textBox.BorderBrush = Brushes.Blue;
            }
            else if (element is Canvas canvas && canvas != _mainCanvas)
            {
                AddOverlayToCanvas(canvas);
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
            else if (element is Polyline polyline)
            {
                polyline.Stroke = properties.Stroke;
                polyline.StrokeThickness = properties.StrokeThickness;
                polyline.StrokeDashArray = properties.StrokeDashArray;
            }
            else if (element is TextBox textBox)
            {
                textBox.BorderThickness = properties.BorderThickness;
                textBox.BorderBrush = properties.BorderBrush;
            }
            else if (element is Canvas canvas && canvas != _mainCanvas)
            {
                RemoveOverlayFromCanvas(canvas);
            }

            _originalProperties.Remove(element);
        }

        public void ClearAllHighlights()
        {
            foreach (var element in new List<UIElement>(_originalProperties.Keys))
            {
                ClearElementHighlight(element);
            }

            foreach (var canvas in new List<Canvas>(_canvasOverlays.Keys))
            {
                RemoveOverlayFromCanvas(canvas);
            }
        }

        private void AddOverlayToCanvas(Canvas canvas)
        {
            if (_canvasOverlays.ContainsKey(canvas)) return;

            var overlayBorder = new Border
            {
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.Blue,
                Background = Brushes.Transparent,
                IsHitTestVisible = false 
            };

            Canvas.SetLeft(overlayBorder, Canvas.GetLeft(canvas));
            Canvas.SetTop(overlayBorder, Canvas.GetTop(canvas));
            overlayBorder.Width = canvas.ActualWidth;
            overlayBorder.Height = canvas.ActualHeight;

            Canvas.SetZIndex(overlayBorder, Canvas.GetZIndex(canvas) + 1);

            _mainCanvas.Children.Add(overlayBorder);
            _canvasOverlays[canvas] = overlayBorder;
            canvas.SizeChanged += OnCanvasSizeChanged;
            canvas.LayoutUpdated += OnCanvasLayoutUpdated;
        }

        private void RemoveOverlayFromCanvas(Canvas canvas)
        {
            if (!_canvasOverlays.ContainsKey(canvas)) return;

            var overlayBorder = _canvasOverlays[canvas];
            canvas.SizeChanged -= OnCanvasSizeChanged;
            canvas.LayoutUpdated -= OnCanvasLayoutUpdated;

            _mainCanvas.Children.Remove(overlayBorder);
            _canvasOverlays.Remove(canvas);
        }

        private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Canvas canvas && _canvasOverlays.ContainsKey(canvas))
            {
                var overlay = _canvasOverlays[canvas];
                overlay.Width = canvas.ActualWidth;
                overlay.Height = canvas.ActualHeight;
            }
        }

        private void OnCanvasLayoutUpdated(object sender, System.EventArgs e)
        {
            if (sender is Canvas canvas && _canvasOverlays.ContainsKey(canvas))
            {
                var overlay = _canvasOverlays[canvas];
                Canvas.SetLeft(overlay, Canvas.GetLeft(canvas));
                Canvas.SetTop(overlay, Canvas.GetTop(canvas));
            }
        }

        public void UpdateStrokeColor(UIElement element, Color color)
        {
            if (element == null) return;

            if (element is Shape shape || element is Polyline)
            {
                if (!_originalProperties.ContainsKey(element))
                {
                    SaveOriginalProperties(element);
                }

                if (element is Shape shapeElement)
                {
                    shapeElement.Stroke = new SolidColorBrush(color);
                }
                else if (element is Polyline polylineElement)
                {
                    polylineElement.Stroke = new SolidColorBrush(color);
                }

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
            public double? CanvasLeft { get; set; }
            public double? CanvasTop { get; set; }
            public int CanvasZIndex { get; set; }
        }
    }
}