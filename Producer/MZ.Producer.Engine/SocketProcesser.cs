using Prism.Mvvm;
using System.Net;
using System;
using System.Reflection;
using System.Net.Sockets;
using System.Threading.Tasks;
using MZ.Logger;
using MZ.Model;

namespace MZ.Producer.Engine
{
    public class SocketProcesser : BindableBase
    {
        #region Model
        public IpNetworkModel Model { get; set; } = new();

        private string _message = string.Empty;
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        #endregion

        #region Params(Network)
        private TcpClient _client;
        private NetworkStream _stream;
        #endregion

        public SocketProcesser()
        {

        }

        public async Task ConnectAsync()
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(IPAddress.Parse(Model.Ip), Model.Port);
                _stream = _client.GetStream();
                Model.IsConnected = true;
                Message = $"IsConnected = {Model.IsConnected}";
            }
            catch (Exception ex)
            {
                Model.IsConnected = false;
                Message = ex.Message;
                MZLogger.Error(ex.Message);
            }
        }


        public void Disconnect()
        {
            _stream?.Close();
            _client?.Close();

            _stream = null;
            _client = null;

            Model.IsConnected = false;

            Message = $"IsConnected = {Model.IsConnected}";
        }

        public async Task<bool> SendAsync(FileModel model)
        {
            try
            {
                var image = model.Image.ToBytes(".tiff");
                var info = $"{model.Name}|{model.Width}|{model.Height}|{image.Length}";
                var infoToByte = System.Text.Encoding.UTF8.GetBytes(info);

                await _stream.WriteAsync(BitConverter.GetBytes(infoToByte.Length).AsMemory(0, 4));
                await _stream.WriteAsync(infoToByte);
                await _stream.WriteAsync(image);

                Message = $"{MethodBase.GetCurrentMethod().Name} = {model.Name}";
                return true;
            }
            catch (Exception ex)
            {
                //Disconnect();
                Message = ex.Message;
                MZLogger.Error(ex.Message);
                return false;
            }
        }
    }
}
