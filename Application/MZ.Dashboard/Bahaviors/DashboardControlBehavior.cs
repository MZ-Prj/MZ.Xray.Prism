using MahApps.Metro.IconPacks;
using Microsoft.Xaml.Behaviors;
using MZ.Dashboard.ViewModels;
using MZ.Util;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        private ScaleTransform _scaleTransform;

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
            
            var pickerButton = viewModel.VideoButtons.FirstOrDefault(vb => vb.Command == viewModel.PickerCommand);
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
                var predict = MZFramework.FindChildByName(this.AssociatedObject, "CanvasAIPredictView") as Canvas;
                var zeffect = MZFramework.FindChildByName(this.AssociatedObject, "CanvasZeffectView") as Canvas;

                if (canvas != null)
                {
                    viewModel.CanvasImageView = canvas;
                    viewModel.CanvasPredictView = predict;
                    viewModel.CanvasZeffectView = zeffect;
                }
            }
        }
    }
}
