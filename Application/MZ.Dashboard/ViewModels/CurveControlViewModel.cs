using MZ.Core;
using MZ.Domain.Models;
using MZ.Util;
using MZ.Xray.Engine;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace MZ.Dashboard.ViewModels
{
    public class CurveControlViewModel : MZBindableBase
    {
        #region Service
        private readonly IXrayService _xrayService;

        public MediaProcesser Media
        { 
            get => _xrayService.Media; 
            set => _xrayService.Media = value; 
        }

        public CurveSplineProcesser CurveSpline
        {
            get => _xrayService.CurveSpline;
            set => _xrayService.CurveSpline = value;
        }
        #endregion

        #region Params
        public ObservableCollection<IconButtonModel> WindowCommandButtons { get; } = [];

        #endregion

        #region Event
        public event Action<Point> ClearPoints;
        public event Action UpdatePoints;

        #endregion


        #region Commands
        private DelegateCommand<CurveControlModel> _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new DelegateCommand<CurveControlModel>(MZAction.Wrapper<CurveControlModel>(DeleteButton));

        #endregion

        public CurveControlViewModel(IContainerExtension container, IXrayService xrayService) : base(container)
        {
            _xrayService = xrayService;

            base.Initialize();
        }

        public override void InitializeModel()
        {
        }

        /// <summary>
        /// 물성분석 색상 항목 삭제
        /// </summary>
        /// <param name="model">Point : 삭제 항목</param>
        private void DeleteButton(CurveControlModel model)
        {
            CurveSpline.Points.Remove(model);

            InvokeClearPoints(model);
        }


        /// <summary>
        /// Behavior -> ViewModel
        /// </summary>
        public void InvokeClearPoints(CurveControlModel model)
        {
            ClearPoints?.Invoke(new((int)model.X, (int)model.Y));
        }

        public void InvokeUpdatePoints()
        {
            UpdatePoints?.Invoke();
        }
    }
}
