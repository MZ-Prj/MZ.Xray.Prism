using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Prism.Ioc;
using MZ.Core;
using MZ.Logger;
using MZ.Auth.Views;
using MZ.Blank.Views;
using MZ.Xray.Engine;
using MZ.Infrastructure;
using static MZ.Core.MZModel;
using static MZ.Event.MZEvent;
using MZ.AI.Engine;

namespace MZ.Splash.ViewModels
{
    public class SplashWindowViewModel : MZBindableBase
    {
        #region Service
        private readonly IDatabaseService _databaseService;
        private readonly IXrayService _xrayService;
        #endregion

        #region Params
        private List<Func<Task>> _process = [];

        private int _maxStep = 0;
        public int MaxStep { get => _maxStep; set => SetProperty(ref _maxStep, value); }

        private int _step = 0;
        public int Step { get => _step; set => SetProperty(ref _step, value); }

        private string _message = string.Empty;
        public string Message { get => _message; set => SetProperty(ref _message, value); }
        #endregion
        public SplashWindowViewModel(IContainerExtension container, IXrayService xrayService, IDatabaseService databaseService) : base(container)
        {
            _databaseService = databaseService;
            _xrayService = xrayService;
            base.Initialize();

        }

        public override void InitializeCore()
        {
            _dispatcher.Invoke(async () =>
            {
                var steps = new (string Message, Func<Task> Action)[]
                {
                    ("Loading...", async () =>
                    {
                        await Task.CompletedTask;
                    }),
                    ("Initialize Database", async () =>
                    {
                        await _databaseService.MakeAdmin();
                        await Task.CompletedTask;
                    }),
                    ("Initialize Network", async () =>
                    {
                        _xrayService.InitializeSocket();
                        await Task.CompletedTask;
                    }),
                    ("Initialize AI", async () =>
                    {
                        await _xrayService.InitializeAI();
                        await Task.CompletedTask;
                    }),
                    ("Success!", async () =>
                    {
                        _eventAggregator.GetEvent<SplashCloseEvent>().Publish();
                        _eventAggregator.GetEvent<DashboardNavigationEvent>().Publish(
                            new NavigationModel(
                                MZRegionNames.DashboardRegion,
                                nameof(UserLoginView)));

                        await Task.CompletedTask;
                    })
                };

                _process = [.. steps.Select((step, index) => new Func<Task>(async () =>
                {
                    await CallbackMessage(index, step.Message);
                    await step.Action();
                }))];

                MaxStep = _process.Count - 1;

                foreach (var process in _process)
                {
                    await process();
                }
            });
        }

        /// <summary>
        /// Callback Message
        /// </summary>
        /// <param name="step"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task CallbackMessage(int step, string message)
        {
            _dispatcher.Invoke(() =>
            {
                Step = step;
                Message = $"{message} [{step}/{MaxStep}]";
            });
            MZLogger.Information($"{message}");
            await Task.Delay(100);
        }
    }
}
