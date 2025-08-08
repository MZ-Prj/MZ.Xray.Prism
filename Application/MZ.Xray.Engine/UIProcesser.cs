using MZ.Domain.Entities;
using MZ.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// 사용자별로 버튼 상태(노출 여부 등)를 UI에 반영해주는 프로세서
    /// </summary>
    public partial class UIProcesser : BindableBase
    {

        private ObservableCollection<IconButtonModel> _actionButtons = [];
        public ObservableCollection<IconButtonModel> ActionButtons { get => _actionButtons; set => SetProperty(ref _actionButtons, value); }
        
        /// <summary>
        /// DB에서 불러온 버튼 설정값을 UI용 버튼 모델에 매핑/반영
        /// </summary>
        public void LoadActionButton(IEnumerable<UserButtonEntity> buttonSettings)
        {
            if (buttonSettings == null)
            {
                return;
            }

            ActionButtons.ToList().ForEach(button =>
            {
                var setting = buttonSettings.FirstOrDefault(b => b.Name == button.Name);
                button.Id = setting.Id;
                button.IsVisibility = setting?.IsVisibility ?? true;
            });
        }

    }

    /// <summary>
    /// 실시간 사용자 프로그램 사용 시간 조회
    /// </summary>
    public partial class UIProcesser : BindableBase
    {
        private TimeSpan _usingDate;
        public TimeSpan UsingDate { get => _usingDate; set => SetProperty(ref _usingDate, value); }

        private DispatcherTimer _usingDateTimer;

        public void StartUsingDate(TimeSpan usingDate, DateTime loginTime)
        {
            Cleanup();

            UsingDate = usingDate + (DateTime.Now - loginTime);

            _usingDateTimer = new DispatcherTimer();
            _usingDateTimer.Tick += OnUsingDateTimerElapsed;
            _usingDateTimer.Interval = TimeSpan.FromSeconds(1);
            _usingDateTimer.Start();
        }

        private void OnUsingDateTimerElapsed(object sender, EventArgs e)
        {
            UsingDate = UsingDate.Add(TimeSpan.FromSeconds(1));
        }

        public void Cleanup()
        {
            if (_usingDateTimer != null)
            {
                _usingDateTimer.Stop();
                _usingDateTimer.Tick -= OnUsingDateTimerElapsed;
                _usingDateTimer = null;
            }
        }
    }

    public partial class UIProcesser : BindableBase
    {
        private string _splashMessage = string.Empty;
        public string SplashMessage { get => _splashMessage; set => SetProperty(ref _splashMessage, value); }
    }
}