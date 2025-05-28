using System.Windows.Media;
using System.Windows;
using SharpVectors.Converters;

namespace MZ.Resource.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class SvgManager : SvgViewbox
    {
        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(SvgManager), new PropertyMetadata(Brushes.Black, OnFillChanged));

        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SvgManager svgManager)
            {
                svgManager.UpdateFill((Brush)e.NewValue);
            }
        }

        private void UpdateFill(Brush newFill)
        {
            if (Drawings != null)
            {
                UpdateDrawingBrush(Drawings, newFill);
            }
        }

        private void UpdateDrawingBrush(Drawing drawing, Brush newFill)
        {
            if (drawing is DrawingGroup drawingGroup)
            {
                foreach (var child in drawingGroup.Children)
                {
                    UpdateDrawingBrush(child, newFill);
                }
            }
            else if (drawing is GeometryDrawing geometryDrawing)
            {
                geometryDrawing.Brush = newFill;
            }
        }
    }
}
