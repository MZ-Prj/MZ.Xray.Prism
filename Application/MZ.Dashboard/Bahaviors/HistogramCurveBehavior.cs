using Microsoft.Xaml.Behaviors;
using MZ.Dashboard.ViewModels;
using MZ.Vision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MZ.Dashboard.Bahaviors
{
    /// <summary>
    /// Canvas에 실시간 Xray 화면 Histogram 및 Curve 컨트롤 Behavior 클래스
    /// </summary>
    public class HistogramCurveBehavior : Behavior<Canvas>
    {
        private readonly List<Ellipse> _points = [];
        private Ellipse _draggingPoint;
        private DateTime _lastUpdate = DateTime.MinValue;

        public HistogramCurveBehavior()
        {
        }

        /// <summary>
        /// Behavior OnAttached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseRightButtonDown += OnMouseRightButtonDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;

            if (AssociatedObject.DataContext is CurveControlViewModel viewModel)
            {
                viewModel.ClearPoints += RemovePoint;
                viewModel.UpdatePoints += UpdateCurve;
            }
        }


        /// <summary>
        /// Behavior OnDetaching
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseRightButtonDown -= OnMouseRightButtonDown;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;

            if (AssociatedObject.DataContext is CurveControlViewModel viewModel)
            {
                viewModel.ClearPoints -= RemovePoint;
                viewModel.UpdatePoints -= UpdateCurve;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject.DataContext is CurveControlViewModel viewModel)
            {
                var points = viewModel.CurveSpline.Points.ToList();

                foreach (var point in points)
                {
                    AddPoint(new(point.X, point.Y));
                }
            }
        }
        /// <summary>
        /// 마우스 왼쪽 버튼(추가)
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = AssociatedObject;
            var position = e.GetPosition(canvas);

            _draggingPoint = GetPointAtPosition(position);

            if (_draggingPoint == null)
            {
                AddPoint(position);
            }
        }

        /// <summary>
        /// 마우스 오른쪽 버튼(삭제)
        /// </summary>
        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = AssociatedObject;
            var position = e.GetPosition(canvas);

            RemovePoint(position);
        }

        /// <summary>
        /// 마우스 이동(원점 좌표 이동)
        /// </summary>
        
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_draggingPoint != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var now = DateTime.Now;
                if ((now - _lastUpdate).TotalMilliseconds < 33)
                {
                    return;
                }
                _lastUpdate = now;

                var position = e.GetPosition(AssociatedObject);

                position.X = Math.Clamp(position.X, 0, AssociatedObject.ActualWidth);
                position.Y = Math.Clamp(position.Y, 0, AssociatedObject.ActualHeight);

                Canvas.SetLeft(_draggingPoint, position.X - _draggingPoint.Width / 2);
                Canvas.SetTop(_draggingPoint, position.Y - _draggingPoint.Height / 2);

                RemoveDuplicatePoints();
                SortPointsByX();
                UpdateCurve();
            }
        }

        /// <summary>
        /// 드레그 좌표 초기화
        /// </summary>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _draggingPoint = null;
        }

        /// <summary>
        /// 마우스 위치 좌표 추가
        /// </summary>
        private void AddPoint(Point position)
        {
            position.X = Math.Clamp(position.X, 0, AssociatedObject.ActualWidth);
            position.Y = Math.Clamp(position.Y, 0, AssociatedObject.ActualHeight);

            var ellipse = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Gray,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetLeft(ellipse, position.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, position.Y - ellipse.Height / 2);

            AssociatedObject.Children.Add(ellipse);
            _points.Add(ellipse);

            RemoveDuplicatePoints();
            SortPointsByX();
            UpdateCurve();
        }

        /// <summary>
        /// 포인트 삭제
        /// </summary>
        private void RemovePoint(Point position)
        {
            var pointToRemove = GetPointAtPosition(position);
            if (pointToRemove != null)
            {
                AssociatedObject.Children.Remove(pointToRemove);
                _points.Remove(pointToRemove);
                UpdateCurve();
            }
        }

        private void RemoveDuplicatePoints()
        {
            var unique = _points.GroupBy(e => new
                                    {
                                        X = Canvas.GetLeft(e) + e.Width / 2,
                                        Y = Canvas.GetTop(e) + e.Height / 2
                                    })
                                .Select(g => g.First())
                                .ToList();

            foreach (var duplicate in _points.Except(unique).ToList())
            {
                AssociatedObject.Children.Remove(duplicate);
            }

            _points.Clear();
            _points.AddRange(unique);
        }

        /// <summary>
        /// 포인트 정렬 수행
        /// </summary>
        private void SortPointsByX()
        {
            _points.Sort((a, b) =>
            {
                double aX = Canvas.GetLeft(a) + a.Width / 2;
                double bX = Canvas.GetLeft(b) + b.Width / 2;
                return aX.CompareTo(bX);
            });
        }

        /// <summary>
        /// 커브 좌표에 따른 원 출력
        /// </summary>
        private Ellipse GetPointAtPosition(Point position)
        {
            foreach (var point in _points)
            {
                var left = Canvas.GetLeft(point);
                var top = Canvas.GetTop(point);

                if (position.X >= left && position.X <= left + point.Width &&
                    position.Y >= top && position.Y <= top + point.Height)
                {
                    return point;
                }
            }

            return null;
        }

        /// <summary>
        /// 커브 상태 업데이트 수행
        /// </summary>
        
        
        public void UpdateCurve()
        {
            var curves = AssociatedObject.Children.OfType<Polyline>().ToList();
            foreach (var curve in curves)
            {
                AssociatedObject.Children.Remove(curve);
            }

            var points = new List<Point>
            {
                new(0, 0)
            };

            points.AddRange(_points.Select(p => new Point(
                Canvas.GetLeft(p) + p.Width / 2,
                Canvas.GetTop(p) + p.Height / 2)));

            points.Add(new Point(AssociatedObject.ActualWidth, AssociatedObject.ActualHeight));

            if (this.AssociatedObject.DataContext is CurveControlViewModel viewModel)
            {
                viewModel.CurveSpline.Points.Clear();
                foreach (var point in points)
                {
                    viewModel.CurveSpline.AddPoint(new() { X=point.X, Y=point.Y});
                }
            }

            var cvPoints = points.Select(p => new OpenCvSharp.Point(p.X, p.Y)).ToList();

            var splinePoints = VisionBase.GenerateSpline(cvPoints);
            var polyline = new Polyline
            {
                Points = [..splinePoints.Select(p => new Point(p.X, p.Y))],
                Stroke = Brushes.Gray,
                StrokeThickness = 2
            };

            AssociatedObject.Children.Add(polyline);
        }

        /// <summary>
        /// 모든 좌표 초기화
        /// </summary>
        public void ClearPoints()
        {
            foreach (var point in _points)
            {
                AssociatedObject.Children.Remove(point);
            }
            _points.Clear();

            var curves = AssociatedObject.Children.OfType<Polyline>().ToList();
            foreach (var curve in curves)
            {
                AssociatedObject.Children.Remove(curve);
            }

        }
    }
}
