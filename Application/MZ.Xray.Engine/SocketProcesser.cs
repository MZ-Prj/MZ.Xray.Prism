using MZ.Domain.Models;
using MZ.DTO.Enums;
using MZ.Util;
using OpenCvSharp;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static MZ.Event.MZEvent;

namespace MZ.Xray.Engine
{
    public class SocketReceiveProcesser : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        #region Params
        private TcpListener _listener;
        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cancellationTokenSource;
        public IpNetworkModel Model { get; set; } = new();
        #endregion

        public SocketReceiveProcesser()
        {

        }

        public SocketReceiveProcesser(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Create()
        {
            try
            {
                _cancellationTokenSource = new();
                _listener = new TcpListener(IPAddress.Any, Model.Port);
                _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _listener.Start();
            }
            catch 
            {
            }
        }

        public async Task ReceiveAsync()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                _client?.Dispose();
                _stream?.Dispose();

                _client = await _listener.AcceptTcpClientAsync();
                _stream = _client.GetStream();

                while (true)
                {
                    var model = await ReceiveXrayDataAsync();
                    if (model == null)
                    {
                        break;
                    }
                    _eventAggregator.GetEvent<FileReceiveEvent>().Publish(model);
                }
            }
        }

        public void Dispose()
        {
            _stream?.Close();
            _client?.Close();
            _listener?.Stop();
        }


        /// <summary>
        /// Recevie Xray Data
        /// note : 직렬화 적용 -> 다른 언어일때 될 때도 안될때도 있어서 제외(호환성 문제)
        /// 실 사용할때는 외부장비에서 데이터를 받아오기 때문에 직접 변형 수행
        /// 직렬화를 적용시킬 경우 MessagePack 사용
        /// code : var dto = MessagePackSerializer.Deserialize<FileModel>(serializedData);
        /// </summary>
        /// <returns></returns>
        public async Task<FileModel> ReceiveXrayDataAsync()
        {
            try
            {
                //Header Length (4byte)
                byte[] headerSizeBuffer = new byte[4];
                await _stream.ReadAsync(headerSizeBuffer.AsMemory(0, 4));
                int headerSize = BitConverter.ToInt32(headerSizeBuffer, 0);
                if (headerSize == 0)
                {
                    return null;
                }

                //Header Information (utf-8)
                byte[] headerBuffer = new byte[headerSize];
                await _stream.ReadAsync(headerBuffer.AsMemory(0, headerSize));
                string header = System.Text.Encoding.UTF8.GetString(headerBuffer);
                string[] parts = header.Split('|');

                //Receive Image
                int imageSize = int.Parse(parts[3]);
                if (imageSize == 0)
                {
                    return null;
                }

                byte[] imageBuffer = new byte[imageSize];
                int totalSize = 0;
                while (totalSize < imageSize)
                {
                    int read = await _stream.ReadAsync(imageBuffer.AsMemory(totalSize, imageSize - totalSize));
                    totalSize += read;
                }

                //Convert Model
                var model = new FileModel
                {
                    Name = parts[0],
                    Width = int.Parse(parts[1]),
                    Height = int.Parse(parts[2]),
                    Image = Cv2.ImDecode(imageBuffer, ImreadModes.Unchanged),
                    Message = MZEnum.GetDescription(BaseRole.Success),
                };

                return model;

            }
            catch
            {
                return null;
            }
        }
    }
}
