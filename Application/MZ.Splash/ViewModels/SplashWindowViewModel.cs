using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MZ.Core;
using Prism.Ioc;
using System.Linq;
using System.Windows;
using static MZ.Core.MZEvent;

namespace MZ.Splash.ViewModels
{
    public class SplashWindowViewModel : MZBindableBase
    {
        private List<Func<Task>> _process = [];

        public SplashWindowViewModel(IContainerExtension container) : base(container)
        {
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
                        _eventAggregator.GetEvent<SplashStatusEvent>().Publish();
                        await Task.CompletedTask;
                    })
                };

                _process = [.. steps.Select((step, index) => new Func<Task>(async () =>
                {
                    await step.Action();
                    await CallbackMessage(index, step.Message);
                }))];

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
            await Task.Delay(1000);
        }

    }
}
