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
    public partial class XrayRealtimeBehavior : Behavior<Canvas>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            OnAttachedLoaded();
            OnAttachedMouse();
            OnAttachedScale();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            OnDetachingLoaded();
            OnDetachingMouse();
            OnDetachingScale();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class XrayRealtimeBehavior : Behavior<Canvas>
    {
        private ScaleTransform _scaleTransform;
        private void OnAttachedScale()
        {
            _scaleTransform = new(1.0, 1.0);
            this.AssociatedObject.RenderTransform = _scaleTransform;
        }

        private void OnDetachingScale()
        {
            this.AssociatedObject.RenderTransform = null;
        }
    }

    public partial class XrayRealtimeBehavior : Behavior<Canvas>
    {
        private void OnAttachedMouse()
        {
            this.AssociatedObject.MouseEnter += OnMouseEnter;
            this.AssociatedObject.MouseLeave += OnMouseLeave;
        }
        private void OnDetachingMouse()
        {
            this.AssociatedObject.MouseEnter -= OnMouseEnter;
            this.AssociatedObject.MouseLeave -= OnMouseLeave;
        }

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

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var viewModel = this.AssociatedObject.DataContext as XrayRealtimeViewModel;
            
            var pickerButton = viewModel.VideoButtons.FirstOrDefault(vb => vb.Command == viewModel.PickerCommand);
            bool isPinned = pickerButton?.IconKind == nameof(PackIconMaterialKind.Pin);

            SetOverlayVisibility(MZFramework.FindControls(this.AssociatedObject, "OverlayTopControls"), isPinned);
            SetOverlayVisibility(MZFramework.FindControls(this.AssociatedObject, "OverlayBottomControls"), isPinned);
            SetOverlayVisibility(MZFramework.FindControls(this.AssociatedObject, "OverlayGradient"), isPinned);
        }

        private void SetOverlayVisibility(UIElement overlay, bool isVisible)
        {
            if (overlay != null)
            {
                overlay.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }

    public partial class XrayRealtimeBehavior : Behavior<Canvas>
    {
        private void OnAttachedLoaded()
        {
            this.AssociatedObject.Loaded += OnLoaded;
            this.AssociatedObject.LayoutUpdated += OnLayoutUpdated;
        }

        private void OnDetachingLoaded()
        {
            this.AssociatedObject.Loaded -= OnLoaded;
            this.AssociatedObject.LayoutUpdated -= OnLayoutUpdated;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.AssociatedObject.DataContext is XrayRealtimeViewModel viewModel)
            {
                double width = Math.Max(this.AssociatedObject.ActualWidth, 1.0);
                double height = Math.Max(this.AssociatedObject.ActualHeight, 1.0);

                //viewModel.VisionService.ViewParameters.CanvasSize = new(width, height);
                var canvas = MZFramework.FindChildByName(this.AssociatedObject, "CanvasImageView") as Canvas;
                var predict = MZFramework.FindChildByName(this.AssociatedObject, "CanvasAIPredictView") as Canvas;

                if (canvas != null)
                {
                    viewModel.CanvasImageView = canvas;
                    viewModel.CanvasPredictView = predict;
                }
            }
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (this.AssociatedObject.DataContext is XrayRealtimeViewModel viewModel)
            {
                double width = Math.Max(this.AssociatedObject.ActualWidth, 1.0);
                double height = Math.Max(this.AssociatedObject.ActualHeight, 1.0);

                //if (viewModel.VisionService.ViewParameters.CanvasSize.Width != width || viewModel.VisionService.ViewParameters.CanvasSize.Height != height)
                //{
                //    viewModel.VisionService.ViewParameters.CanvasSize = new(width, height);
                //}
            }
        }
    }

}
