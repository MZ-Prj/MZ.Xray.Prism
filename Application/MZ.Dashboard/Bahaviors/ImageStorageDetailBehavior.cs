using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Linq;
using MZ.Dashboard.ViewModels;

namespace MZ.Dashboard.Bahaviors
{
    /// <summary>
    /// 이미지 스토리지 화면 동작 Behavior 클래스
    /// </summary>
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

        public ImageStorageDetailBehavior()
        {

        }
        /// <summary>
        /// Behavior OnAttached
        /// </summary>
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

        /// <summary>
        /// Behavior OnDetaching
        /// </summary>
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

        /// <summary>
        /// 초기 좌표 및 줌 설정
        /// </summary>
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

        /// <summary>
        /// 마우스 휠 동작시 줌 설정
        /// </summary>
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

        /// <summary>
        /// 왼쪽 버튼 클릭시 드레그 드롭 설정(시작)
        /// </summary>
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

        /// <summary>
        /// 왼쪽 버튼 클릭시 드레그 드롭 설정(해제)
        /// </summary>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            AssociatedObject.ReleaseMouseCapture();
        }

        /// <summary>
        /// 왼쪽 버튼 클릭시 드레그 드롭 설정(이동)
        /// </summary>
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


        /// <summary>
        /// 컨버스 크기 바뀌면 좌표 재설정
        /// </summary>
        private void ParentCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CenterCanvas();
        }


        /// <summary>
        /// 좌표 중앙 설정 수행
        /// </summary>
        private void CenterCanvas()
        {
            if (_parentCanvas != null)
            {
                if (AssociatedObject is FrameworkElement frameworkElement && frameworkElement.ActualWidth > 0)
                {
                    double scale = _scaleTransform.ScaleX; 
                    double scaledWidth = frameworkElement.ActualWidth * scale;
                    double scaledHeight = frameworkElement.ActualHeight * scale;

                    double offsetX = (_parentCanvas.ActualWidth - scaledWidth) / 2;
                    double offsetY = (_parentCanvas.ActualHeight - scaledHeight) / 2;

                    _translateTransform.X = offsetX;
                    _translateTransform.Y = offsetY;
                }
            }
        }

        /// <summary>
        /// 커서 
        /// </summary>
        /// <param name="cursor"></param>
        private void OnEventCursor(Cursor cursor)
        {
            if (AssociatedObject is FrameworkElement frameworkElement)
            {
                frameworkElement.Cursor = cursor;
            }
        }


        /// <summary>
        /// 새로고침 
        /// </summary>
        private void OnEventRefresh()
        {
            
            _scaleTransform.ScaleX = MinZoom;
            _scaleTransform.ScaleY = MinZoom;

            CenterCanvas();
            OnEventCursor(Cursors.Arrow);
        }

        /// <summary>
        /// 줌 갱신
        /// </summary>
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
