using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Prism.Ioc;
using MZ.Core;
using MZ.Logger;
using static MZ.Event.MZEvent;

namespace MZ.Splash.ViewModels
{
    public class SplashWindowViewModel : MZBindableBase
    {
        private List<Func<Task>> _process = [];

        private int _maxStep = 0;
        public int MaxStep { get => _maxStep; set => SetProperty(ref _maxStep, value); }

        private int _step = 0;
        public int Step { get => _step; set => SetProperty(ref _step, value); }

        private string _message = string.Empty;
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        public SplashWindowViewModel(IContainerExtension container) : base(container)
        {
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
                    ("Success!", async () =>
                    {
                        _eventAggregator.GetEvent<SplashCloseEvent>().Publish();

                        await Task.CompletedTask;
                    })
                };

                _process = [.. steps.Select((step, index) => new Func<Task>(async () =>
                {
                    await step.Action();
                    await CallbackMessage(index, step.Message);
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
            await Task.Delay(1000);
        }
    }
}
