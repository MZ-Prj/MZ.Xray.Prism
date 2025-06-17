using MZ.Domain.Models;
using Prism.Mvvm;
using System.Net;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using MZ.Logger;

namespace MZ.Producer.Engine
{
    public class SocketProcesser : BindableBase
    {
        #region Model
        public IpNetworkModel Model { get; set; } = new();
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
            }
            catch (Exception ex)
            {
                Model.IsConnected = false;
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

                return true;
            }
            catch (Exception ex)
            {
                Disconnect();
                MZLogger.Error(ex.Message);
                return false;
            }
        }
    }
}
