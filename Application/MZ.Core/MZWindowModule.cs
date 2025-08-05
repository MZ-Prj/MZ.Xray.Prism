using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using Prism.Ioc;
using MZ.Logger;
using WinForms = System.Windows.Forms;

namespace MZ.Core
{
    /// <summary>
    /// 윈도우 관리용 모듈 베이스 클래스
    /// - DI 컨테이너를 이용해 WPF Window 인스턴스를 생성 및 관리
    /// - 여러 창(윈도우)의 생성/위치 지정/표시/숨김/닫기 등을 통합적으로 관리
    /// - Region과 View 탐색(Navigation) 기능도 포함
    /// </summary>
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
        /// Window 생성 후 Region/View 등록 및 Region Navigation 수행
        /// </summary>
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
                MZLogger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 특정 윈도우를 다중 모니터 중 지정된 screenIndex의 위치 및 크기로 이동
        /// </summary>
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
                MZLogger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Dictionary에 저장된 Window 인스턴스를 반환
        /// </summary>
        public Window GetWindow(string windowKey)
        {
            return Windows.TryGetValue(windowKey, out var window) ? window : null;
        }

        /// <summary>
        /// 해당 windowKey로 등록된 Window를 화면에 표시 (Show)
        /// </summary>
        public void ShowWindow(string windowKey)
        {
            if (Windows.TryGetValue(windowKey, out var window))
            {
                window.Show();
            }
        }

        /// <summary>
        /// 해당 windowKey로 등록된 Window를 닫음 (Close)
        /// </summary>
        public void CloseWindow(string windowKey)
        {
            if (Windows.TryGetValue(windowKey, out var window))
            {
                window.Close();
            }
        }

        /// <summary>
        /// 해당 windowKey로 등록된 Window를 숨김 (Hide)
        /// </summary>
        public void HideWindow(string windowKey)
        {
            if (Windows.TryGetValue(windowKey, out var window))
            {
                window.Hide();
            }
        }
    }
}