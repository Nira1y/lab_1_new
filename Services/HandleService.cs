using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Services
{
    public class HandleService
    {
        private readonly ResizeService _resizeService;

        public HandleService()
        {
            _resizeService = new ResizeService();
        }

        public void CreateResizeHandles(Canvas canvas, UIElement selectedElement)
        {
            if (canvas == null || selectedElement == null) return;

            RemoveResizeHandles(canvas);

            Rect bounds = _resizeService.GetElementBounds(selectedElement);

            CreateResizeHandle(canvas, bounds.TopLeft, ResizeService.ResizeHandle.TopLeft);
            CreateResizeHandle(canvas, bounds.TopRight, ResizeService.ResizeHandle.TopRight);
            CreateResizeHandle(canvas, bounds.BottomLeft, ResizeService.ResizeHandle.BottomLeft);
            CreateResizeHandle(canvas, bounds.BottomRight, ResizeService.ResizeHandle.BottomRight);
            CreateResizeHandle(canvas, new Point(bounds.Left + bounds.Width / 2, bounds.Top), ResizeService.ResizeHandle.Top);
            CreateResizeHandle(canvas, new Point(bounds.Left + bounds.Width / 2, bounds.Bottom), ResizeService.ResizeHandle.Bottom);
            CreateResizeHandle(canvas, new Point(bounds.Left, bounds.Top + bounds.Height / 2), ResizeService.ResizeHandle.Left);
            CreateResizeHandle(canvas, new Point(bounds.Right, bounds.Top + bounds.Height / 2), ResizeService.ResizeHandle.Right);
        }

        private void CreateResizeHandle(Canvas canvas, Point position, ResizeService.ResizeHandle handleType)
        {
            if (canvas == null) return;

            var handle = new Rectangle
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Tag = handleType
            };

            Canvas.SetLeft(handle, position.X - 4);
            Canvas.SetTop(handle, position.Y - 4);
            canvas.Children.Add(handle);
        }

        public ResizeService.ResizeHandle GetResizeHandleAtPosition(Point position, Canvas canvas)
        {
            if (canvas == null) return ResizeService.ResizeHandle.None;

            foreach (var child in canvas.Children)
            {
                if (child is Rectangle handle && handle.Tag is ResizeService.ResizeHandle handleType)
                {
                    double left = Canvas.GetLeft(handle);
                    double top = Canvas.GetTop(handle);

                    Rect handleBounds = new Rect(left, top, handle.Width, handle.Height);

                    Rect expandedBounds = new Rect(
                        handleBounds.X - 2,
                        handleBounds.Y - 2,
                        handleBounds.Width + 4,
                        handleBounds.Height + 4
                    );

                    if (expandedBounds.Contains(position))
                    {
                        return handleType;
                    }
                }
            }
            return ResizeService.ResizeHandle.None;
        }

        public void RemoveResizeHandles(Canvas canvas)
        {
            if (canvas == null) return;

            var handlesToRemove = new List<UIElement>();
            foreach (var child in canvas.Children)
            {
                if (child is Rectangle handle && handle.Tag is ResizeService.ResizeHandle)
                {
                    handlesToRemove.Add(handle);
                }
            }

            foreach (var handle in handlesToRemove)
            {
                canvas.Children.Remove(handle);
            }
        }

        public bool IsResizeHandle(UIElement element)
        {
            return element is Rectangle rect && rect.Tag is ResizeService.ResizeHandle;
        }
    }
}