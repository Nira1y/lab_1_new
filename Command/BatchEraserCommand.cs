using lab_2_graphic_editor.Commands;
using lab_2_graphic_editor.Models.Tools;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace lab_2_graphic_editor.Commands
{
    public class BatchEraserCommand : ICommand
    {
        private readonly List<UIElement> _erasedElements;
        private readonly BitmapEraseSession _bitmapSession;
        private readonly Canvas _canvas;

        public BatchEraserCommand(List<UIElement> erasedElements, BitmapEraseSession bitmapSession, Canvas canvas)
        {
            _erasedElements = new List<UIElement>(erasedElements);
            _bitmapSession = bitmapSession;
            _canvas = canvas;
        }

        public void Execute()
        {
            foreach (var element in _erasedElements)
            {
                if (_canvas.Children.Contains(element))
                {
                    _canvas.Children.Remove(element);
                }
            }

            if (_bitmapSession != null && _bitmapSession.HasChanges)
            {
                _bitmapSession.ApplyFinalBitmap();
            }
        }

        public void Undo()
        {
            foreach (var element in _erasedElements)
            {
                if (!_canvas.Children.Contains(element))
                {
                    _canvas.Children.Add(element);
                }
            }

            if (_bitmapSession != null && _bitmapSession.HasChanges)
            {
                _bitmapSession.RestoreOriginalBitmap();
            }
        }
    }
}