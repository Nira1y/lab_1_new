using System.Windows;
using System.Windows.Controls;

namespace lab_2_graphic_editor.Services
{
    public class ZOrderService
    {
        public void BringToFront(Canvas canvas, UIElement element)
        {
            int maxZIndex = GetMaxZIndex(canvas);
            Panel.SetZIndex(element, maxZIndex + 1);
        }

        public void SendToBack(Canvas canvas, UIElement element)
        {
            int minZIndex = GetMinZIndex(canvas);
            Panel.SetZIndex(element, minZIndex - 1);
        }

        private int GetMaxZIndex(Canvas canvas)
        {
            int maxZ = 0;
            foreach (UIElement child in canvas.Children)
            {
                int zIndex = Panel.GetZIndex(child);
                if (zIndex > maxZ) maxZ = zIndex;
            }
            return maxZ;
        }

        private int GetMinZIndex(Canvas canvas)
        {
            int minZ = 0;
            foreach (UIElement child in canvas.Children)
            {
                int zIndex = Panel.GetZIndex(child);
                if (zIndex < minZ) minZ = zIndex;
            }
            return minZ;
        }
    }
}