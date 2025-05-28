using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using Prism.Ioc;
using MZ.Logger;
using WinForms = System.Windows.Forms;

namespace MZ.Core
{
    public class MZWindowModule : MZModule
    {
        protected Dictionary<string, Window> Windows { get; } = [];

        public MZWindowModule(IContainerExtension container) : base(container)
        {
            InitializeEvent();
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public override void InitializeEvent()
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <param name="regionsAndViews"></param>
        public void SetRegion<TWindow>(string windowKey, params (string regionName, string viewName)[] regionsAndViews) where TWindow : Window
        {
            try
            {
                var window = _container.Resolve<TWindow>();
                Windows[windowKey] = window;

                InitializeRegion(window);
                foreach (var (regionName, viewName) in regionsAndViews)
                {
                    NavigateToRegion(regionName, viewName);
                }

            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
            }
        }

        public void SetWindowLocate(string windowKey, int screenIndex)
        {
            try
            {
                if (Windows.TryGetValue(windowKey, out var window))
                {
                    var screens = WinForms.Screen.AllScreens
                            .OrderBy(s => s.Bounds.Left)
                            .ThenBy(s => s.Bounds.Top)
                            .ToArray();
                    if (screenIndex >= 0 && screenIndex < screens.Length)
                    {
                        var workingArea = screens[screenIndex].WorkingArea;
                        window.Left = workingArea.Left;
                        window.Top = workingArea.Top;
                        window.Width = workingArea.Width;
                        window.Height = workingArea.Height;
                        window.WindowState = WindowState.Normal;
                    }
                }
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowKey"></param>
        /// <returns></returns>
        public Window GetWindow(string windowKey)
        {
            return Windows.TryGetValue(windowKey, out var window) ? window : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowKey"></param>
        public void ShowWindow(string windowKey)
        {
            if (Windows.TryGetValue(windowKey, out var window))
            {
                window.Show();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowKey"></param>
        public void CloseWindow(string windowKey)
        {
            if (Windows.TryGetValue(windowKey, out var window))
            {
                window.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowKey"></param>
        public void HideWindow(string windowKey)
        {
            if (Windows.TryGetValue(windowKey, out var window))
            {
                window.Hide();
            }
        }

    }
}
