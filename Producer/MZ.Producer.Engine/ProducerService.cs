using MZ.Logger;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using MZ.Domain.Models;
using System.Threading;
using System.Windows.Threading;

namespace MZ.Producer.Engine
{
    public class ProducerService : BindableBase, IProducerService
    {
        #region Dispatcher
        protected readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        #endregion

        #region Processer
        public SocketProcesser Socket { get; set; } = new();
        public XrayDataProcesser XrayData { get; set; } = new();
        #endregion

        #region Params
        private CancellationTokenSource _cancellationTokenSource;
        public bool IsPaused { get; set; } = true;
        public int SendInterval { get; set; } = 10;
        public string Path { get; set; }

        #endregion

        public async Task LoadFilesAsync(string path)
        {
            Path = path;

            await Task.Run(() =>
            {
                XrayData.LoadFiles(Path);
            });
        }

        public async Task SendFileAsync(FileModel model)
        {
            try
            {
                if (!Socket.Model.IsConnected)
                {
                    await Socket.ConnectAsync();
                }
                await Socket.SendAsync(model);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.Message);
            }
        }

        public async Task LoadAsync()
        {
            try
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }

                IsPaused = true;

                _cancellationTokenSource = new CancellationTokenSource();

                if (!Socket.Model.IsConnected)
                {
                    await Socket.ConnectAsync();
                }

                if (Socket.Model.IsConnected)
                {
                    await LoadFilesAsync(Path);

                    _cancellationTokenSource?.Dispose();
                    _cancellationTokenSource = new();

                    XrayData.CurrentIndex = 0;
                }

            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.Message);
            }
        }

        public void Stop()
        {
            XrayData.CurrentIndex = 0;
            Socket.Disconnect();
            _cancellationTokenSource?.Cancel();
        }

        public void Pause()
        {
            IsPaused = !IsPaused;
        }

        public async Task RunAsync()
        {
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (IsPaused)
                    {
                        await Task.Delay(SendInterval, _cancellationTokenSource.Token).ConfigureAwait(false);
                        continue;
                    }

                    if (XrayData.Models.Count == 0 || XrayData.CurrentIndex >= XrayData.Models.Count)
                    {
                        break;
                    }

                    FileModel data = XrayData.GetCurrentFile();
                    bool check = await Socket.SendAsync(data).ConfigureAwait(false);

                    XrayData.Models[XrayData.CurrentIndex].Message = check ? "Success" : "Fail";
                    XrayData.CurrentIndex++;

                    await Task.Delay(SendInterval, _cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MZLogger.Error(ex.Message);
            }
        }

    }
}
