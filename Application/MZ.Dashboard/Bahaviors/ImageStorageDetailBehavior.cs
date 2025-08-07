using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Linq;
using MZ.Dashboard.ViewModels;

namespace MZ.Dashboard.Bahaviors
{
    public class ImageStorageDetailBehavior : Behavior<Canvas>
    {
        private Canvas _parentCanvas;
        private Point _lastMousePosition;
        private bool _isDragging;

        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;
        private TransformGroup _transformGroup;

        private const double ZoomFactor = 1.1;
        private const double MinZoom = 1.0;
        private const double MaxZoom = 10.0;

        protected override void OnAttached()
        {
            base.OnAttached();

            _parentCanvas = VisualTreeHelper.GetParent(AssociatedObject) as Canvas;
            InitTransforms();

            AssociatedObject.MouseWheel += OnMouseWheel;
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.MouseMove += OnMouseMove;

            if (AssociatedObject.DataContext is ImageStorageDetailViewModel viewModel)
            {
                viewModel.UpdateZoom += OnEventUpdateZoom;
                viewModel.Refresh += OnEventRefresh;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseWheel -= OnMouseWheel;
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            AssociatedObject.MouseMove -= OnMouseMove;

            if (AssociatedObject.DataContext is ImageStorageDetailViewModel viewModel)
            {
                viewModel.UpdateZoom += OnEventUpdateZoom;
                viewModel.Refresh += OnEventRefresh;
            }
        }

        private void InitTransforms()
        {
            _transformGroup = AssociatedObject.RenderTransform as TransformGroup;

            if (_transformGroup == null)
            {
                _scaleTransform = new ScaleTransform(MinZoom, MinZoom);
                _translateTransform = new TranslateTransform();

                _transformGroup = new TransformGroup();
                _transformGroup.Children.Add(_scaleTransform);
                _transformGroup.Children.Add(_translateTransform);

                AssociatedObject.RenderTransform = _transformGroup;
            }
            else
            {
                _scaleTransform = _transformGroup.Children.OfType<ScaleTransform>().FirstOrDefault()
                                  ?? new ScaleTransform(MinZoom, MinZoom);
                _translateTransform = _transformGroup.Children.OfType<TranslateTransform>().FirstOrDefault()
                                      ?? new TranslateTransform();
            }

            AssociatedObject.RenderTransformOrigin = new Point(0, 0);

            if (_parentCanvas != null)
            {
                _parentCanvas.SizeChanged += ParentCanvas_SizeChanged;
                CenterCanvas();
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_parentCanvas == null)
            {
                return;
            }

            Point point = e.GetPosition(_parentCanvas);


            double scaleDelta = e.Delta > 0 ? ZoomFactor : 1 / ZoomFactor;
            double newScale = _scaleTransform.ScaleX * scaleDelta;

            if (newScale < MinZoom || newScale > MaxZoom)
            {
                return;
            }

            Point relative = new(
                (point.X - _translateTransform.X) / _scaleTransform.ScaleX,
                (point.Y - _translateTransform.Y) / _scaleTransform.ScaleY);

            _scaleTransform.ScaleX = newScale;
            _scaleTransform.ScaleY = newScale;

            _translateTransform.X = point.X - relative.X * newScale;
            _translateTransform.Y = point.Y - relative.Y * newScale;

            OnEventCursor(Cursors.Cross);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _parentCanvas ??= VisualTreeHelper.GetParent(AssociatedObject) as Canvas;
            if (_parentCanvas == null)
            {
                return;
            }

            _lastMousePosition = e.GetPosition(_parentCanvas ?? AssociatedObject);
            _isDragging = true;
            AssociatedObject.CaptureMouse();
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            AssociatedObject.ReleaseMouseCapture();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {

            if (_parentCanvas == null || !_isDragging)
            {
                return;
            }

            Point currentPosition = e.GetPosition(_parentCanvas ?? AssociatedObject);
            Vector delta = currentPosition - _lastMousePosition;
            
            _translateTransform.X += (delta.X / _scaleTransform.ScaleX);
            _translateTransform.Y += (delta.Y / _scaleTransform.ScaleY);

            _lastMousePosition = currentPosition;
        }


        private void ParentCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CenterCanvas();
        }

        private void CenterCanvas()
        {
            if (_parentCanvas != null)
            {
                if (AssociatedObject is FrameworkElement fe && fe.ActualWidth > 0)
                {
                    double offsetX = (_parentCanvas.ActualWidth - fe.ActualWidth) / 2;
                    _translateTransform.X = offsetX;
                }
            }
        }

        private void OnEventCursor(Cursor cursor)
        {
            if (AssociatedObject is FrameworkElement frameworkElement)
            {
                frameworkElement.Cursor = cursor;
            }
        }


        private void OnEventRefresh()
        {
            
            _scaleTransform.ScaleX = MinZoom;
            _scaleTransform.ScaleY = MinZoom;

            _translateTransform.X = 0;
            _translateTransform.Y = 0;

            CenterCanvas(); 
        }

        private void OnEventUpdateZoom(int delta)
        {
            if (_parentCanvas == null || _scaleTransform == null || _translateTransform == null)
            {
                return;
            }

            var canvasCenter = new Point(AssociatedObject.ActualWidth / 2, AssociatedObject.ActualHeight / 2);
            var parentCenter = AssociatedObject.TransformToAncestor(_parentCanvas).Transform(canvasCenter);

            double scaleDelta = delta > 0 ? ZoomFactor : 1 / ZoomFactor;
            double newScale = _scaleTransform.ScaleX * scaleDelta;

            if (newScale < MinZoom || newScale > MaxZoom)
            {
                return;
            }

            Point relative = new(
                (parentCenter.X - _translateTransform.X) / _scaleTransform.ScaleX,
                (parentCenter.Y - _translateTransform.Y) / _scaleTransform.ScaleY);

            _scaleTransform.ScaleX = newScale;
            _scaleTransform.ScaleY = newScale;

            _translateTransform.X = parentCenter.X - relative.X * newScale;
            _translateTransform.Y = parentCenter.Y - relative.Y * newScale;

            OnEventCursor(Cursors.Cross);
        }

    }

}
