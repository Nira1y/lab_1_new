using System.Windows;
using System.Windows.Controls;

namespace lab_2_graphic_editor.Services
{
    public class ZOrderService
    {
        public void BringToFront(Canvas canvas, UIElement element)
        {
            if (canvas == null || element == null) return;

            int maxZIndex = GetMaxZIndex(canvas);
            Panel.SetZIndex(element, maxZIndex + 1);
        }

        public void SendToBack(Canvas canvas, UIElement element)
        {
            if (canvas == null || element == null) return;

            Panel.SetZIndex(element, 0);
            foreach (UIElement child in canvas.Children)
            {
                if (child != element && Panel.GetZIndex(child) >= 0)
                {
                    Panel.SetZIndex(child, Panel.GetZIndex(child) + 1);
                }
            }
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
    }
}