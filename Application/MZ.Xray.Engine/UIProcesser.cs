using MZ.Domain.Entities;
using MZ.Domain.Models;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// 사용자별로 버튼 상태(노출 여부 등)를 UI에 반영해주는 프로세서
    /// </summary>
    public class UIProcesser : BindableBase
    {
        public ObservableCollection<IconButtonModel> ActionButtons { set; get; } = [];

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
}
