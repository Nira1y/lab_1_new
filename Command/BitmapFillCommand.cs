using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace lab_2_graphic_editor.Commands
{
    public class BitmapFillCommand : ICommand
    {
        private readonly Canvas _canvas;
        private readonly BitmapSource _originalBitmap;
        private readonly BitmapSource _filledBitmap;
        private readonly Point _fillPosition;
        private Image _currentImage;

        public BitmapFillCommand(Canvas canvas, BitmapSource originalBitmap, BitmapSource filledBitmap, Point fillPosition)
        {
            _canvas = canvas;
            _originalBitmap = originalBitmap;
            _filledBitmap = filledBitmap;
            _fillPosition = fillPosition;
        }

        public void Execute()
        {
            RemoveExistingFillImage();
            _currentImage = new Image
            {
                Source = _filledBitmap,
                Width = _canvas.ActualWidth,
                Height = _canvas.ActualHeight
            };

            _canvas.Children.Add(_currentImage);
        }

        public void Undo()
        {
            RemoveExistingFillImage();
            if (_originalBitmap != null)
            {
                var originalImage = new Image
                {
                    Source = _originalBitmap,
                    Width = _canvas.ActualWidth,
                    Height = _canvas.ActualHeight
                };
                _canvas.Children.Add(originalImage);
                _currentImage = originalImage;
            }
        }

        private void RemoveExistingFillImage()
        {
            if (_currentImage != null && _canvas.Children.Contains(_currentImage))
            {
                _canvas.Children.Remove(_currentImage);
            }
            for (int i = _canvas.Children.Count - 1; i >= 0; i--)
            {
                if (_canvas.Children[i] is Image image && image.Source is WriteableBitmap)
                {
                    _canvas.Children.RemoveAt(i);
                    break;
                }
            }
        }
    }
}