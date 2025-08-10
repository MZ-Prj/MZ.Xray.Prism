using MahApps.Metro.IconPacks;
using Microsoft.Xaml.Behaviors;
using MZ.Dashboard.ViewModels;
using MZ.Util;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#nullable enable
namespace MZ.Dashboard.Bahaviors
{
    /// <summary>
    /// Canvas에 실시간 Xray 화면 동작 Behavior 클래스
    /// </summary>
    public partial class DashboardControlBehavior : Behavior<Canvas>
    {
        /// <summary>
        /// Behavior OnAttached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            OnAttachedLoaded();
            OnAttachedMouse();
            OnAttachedScale();
            OnAttachedFps();
        }

        /// <summary>
        /// Behavior OnDetaching
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            OnDetachingLoaded();
            OnDetachingMouse();
            OnDetachingScale();
            OnDetachingFps();
        }
    }

    /// <summary>
    /// Canvas의 Zoom In/Out 기능 담당
    /// </summary>
    public partial class DashboardControlBehavior : Behavior<Canvas>
    {
        /// <summary>
        /// ScaleTransform 
        /// </summary>
        private ScaleTransform? _scaleTransform;

        /// <summary>
        /// OnAttached : ScaleTransform 생성 및 RenderTransform 설정
        /// </summary>
        private void OnAttachedScale()
        {
            _scaleTransform = new(1.0, 1.0);
            this.AssociatedObject.RenderTransform = _scaleTransform;
        }
        /// <summary>
        /// OnDetaching 
        /// </summary>
        private void OnDetachingScale()
        {
            this.AssociatedObject.RenderTransform = null;
        }
    }
    /// <summary>
    /// 마우스가 Canvas Layout In/Out에 따라 Overlay Control 조절
    /// </summary>
    public partial class DashboardControlBehavior : Behavior<Canvas>
    {
        /// <summary>
        /// OnAttached : 마우스 이밴트 
        /// </summary>
        private void OnAttachedMouse()
        {
            this.AssociatedObject.MouseEnter += OnMouseEnter;
            this.AssociatedObject.MouseLeave += OnMouseLeave;
        }
        /// <summary>
        /// OnDetaching
        /// </summary>
        private void OnDetachingMouse()
        {
            this.AssociatedObject.MouseEnter -= OnMouseEnter;
            this.AssociatedObject.MouseLeave -= OnMouseLeave;
        }
        /// <summary>
        /// 마우스가 Canvas Layout 위에 있을때 Overlay Control 제어
        /// </summary>
        /// <param name="sender">Receive Object</param>
        /// <param name="e">MouseEventArgs</param>
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {

            var overlayTopControls = MZFramework.FindControls(this.AssociatedObject, "OverlayTopControls");
            if (overlayTopControls != null)
            {
                overlayTopControls.Visibility = Visibility.Visible;
            }

            var overlayBottomControls = MZFramework.FindControls(this.AssociatedObject, "OverlayBottomControls");
            if (overlayBottomControls != null)
            {
                overlayBottomControls.Visibility = Visibility.Visible;
            }

            var overlayGradient = MZFramework.FindControls(this.AssociatedObject, "OverlayGradient");
            if (overlayGradient != null)
            {
                overlayGradient.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// 마우스가 Canvas Layer에서 나갈 때, Pin에 따라 Overlay Control Visibility 유무 결정
        /// </summary>
        /// <param name="sender">Receive Object</param>
        /// <param name="e">MouseEventArgs</param>
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var viewModel = this.AssociatedObject.DataContext as DashboardControlViewModel;
            
            var pickerButton = viewModel?.VideoButtons.FirstOrDefault(vb => vb.Command == viewModel.PickerCommand);
            bool isPinned = pickerButton?.IconKind == nameof(PackIconMaterialKind.Pin);

            SetOverlayVisibility(MZFramework.FindControls(this.AssociatedObject, "OverlayTopControls"), isPinned);
            SetOverlayVisibility(MZFramework.FindControls(this.AssociatedObject, "OverlayBottomControls"), isPinned);
            SetOverlayVisibility(MZFramework.FindControls(this.AssociatedObject, "OverlayGradient"), isPinned);
        }
        /// <summary>
        /// Overlay Control Visibility 변경(Visibility to bool)
        /// </summary>
        /// <param name="overlay">UIElement</param>
        /// <param name="isVisible">bool</param>
        private void SetOverlayVisibility(UIElement overlay, bool isVisible)
        {
            if (overlay != null)
            {
                overlay.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
    /// <summary>
    /// Canvas Load/UnLoad 초기 동작 수행
    /// </summary>
    public partial class DashboardControlBehavior : Behavior<Canvas>
    {
        public static readonly DependencyProperty ScreenProperty = DependencyProperty.Register(nameof(Screen), typeof(Canvas), typeof(DashboardControlBehavior), new PropertyMetadata(null));

        public Canvas Screen
        {
            get => (Canvas)GetValue(ScreenProperty);
            set => SetValue(ScreenProperty, value);
        }

        /// <summary>
        /// Attached : Loaded 
        /// </summary>
        private void OnAttachedLoaded()
        {
            this.AssociatedObject.Loaded += OnLoaded;
        }
        /// <summary>
        /// Detaching
        /// </summary>
        private void OnDetachingLoaded()
        {
            this.AssociatedObject.Loaded -= OnLoaded;
        }
        /// <summary>
        /// Canvas가 Load될 때 호출. View-ViewModel의 Canvas 연결부
        /// </summary>
        /// <param name="sender">Receive Object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.AssociatedObject.DataContext is DashboardControlViewModel viewModel)
            {
                int width = (int)Math.Max(this.AssociatedObject.ActualWidth, 1.0);
                int height = (int)Math.Max(this.AssociatedObject.ActualHeight, 1.0);

                viewModel.CreateMedia(width, height);

                var canvas = MZFramework.FindChildByName(this.AssociatedObject, "CanvasImageView") as Canvas;

                if (canvas != null)
                {
                    Screen = canvas;
                }
            }
        }
    }

    /// <summary>
    /// Canvas 하위의 Usercontrol 처리 속도 시간 측정
    /// </summary>
    /// <summary>
    /// Canvas 하위의 Usercontrol 처리 속도 시간 측정
    /// </summary>
    public partial class DashboardControlBehavior
    {
        public static readonly DependencyProperty GenerationProperty = DependencyProperty.Register(nameof(Generation), typeof(int), typeof(DashboardControlBehavior), new PropertyMetadata(0));

        public static readonly DependencyProperty FpsProperty = DependencyProperty.Register(nameof(Fps), typeof(double), typeof(DashboardControlBehavior), new PropertyMetadata(0.0));

        /// <summary>
        /// 새 프레임이 준비됨을 표시, 생산 카운터를 Behavior가 읽음
        /// </summary>
        public int Generation
        {
            get => (int)GetValue(GenerationProperty);
            set => SetValue(GenerationProperty, value);
        }

        /// <summary>
        /// UI 표시 FPS
        /// </summary>
        public double Fps
        {
            get => (double)GetValue(FpsProperty);
            set => SetValue(FpsProperty, value);
        }

        private TimeSpan _lastTimeSpan;
        private readonly Stopwatch _stopwatch = new();
        private int _second;

        /// <summary>
        /// 마지막 렌더에서 본 gen
        /// </summary>
        private int _lastSeenGen;

        /// <summary>
        /// FPS 측정 초기화 & 구독
        /// </summary>
        private void OnAttachedFps()
        {
            _lastTimeSpan = default;
            _second = 0;
            _lastSeenGen = Generation;
            _stopwatch.Restart();

            CompositionTarget.Rendering += OnRenderingFps;
        }

        /// <summary>
        /// FPS 측정 해제
        /// </summary>
        private void OnDetachingFps()
        {
            CompositionTarget.Rendering -= OnRenderingFps;
            _stopwatch.Reset();
        }

        /// <summary>
        /// WPF 렌더 루프 기준으로 UI FPS표시
        /// </summary>
        private void OnRenderingFps(object? sender, EventArgs e)
        {
            // 컨트롤이 화면에 없거나 숨김이면 스킵
            if (this.AssociatedObject == null || !this.AssociatedObject.IsVisible)
            {
                return;
            }

            var args = (RenderingEventArgs)e;
            if (_lastTimeSpan == default)
            {
                _lastTimeSpan = args.RenderingTime;
                return;
            }

            // 이번 렌더와 비교해서 다르면 프래임간주
            int gen = Generation;
            if (gen != _lastSeenGen)
            {
                _lastSeenGen = gen;
                _second++;
            }

            // 1초마다 스냅샷
            if (_stopwatch.ElapsedMilliseconds >= 1000)
            {
                Fps = _second;

                _second = 0;
                _stopwatch.Restart();
            }
        }
    }

}
