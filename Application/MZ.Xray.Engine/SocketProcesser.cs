using MZ.Domain.Models;
using MZ.DTO.Enums;
using MZ.Util;
using OpenCvSharp;
using Prism.Mvvm;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MZ.Xray.Engine
{
    public class SocketReceiveProcesser : BindableBase
    {
        private TcpListener _listener;
        private TcpClient _client;
        private NetworkStream _stream;
        public bool IsConnected { get; set; }
        public int Port { get; set; }

        public async Task Start(int port)
        {
            Port = port; 
            try
            {
                _listener = new TcpListener(IPAddress.Any, port);
                _listener.Start();

                _client = await _listener.AcceptTcpClientAsync();
                _stream = _client.GetStream();

                IsConnected = true;
            }
            catch
            {

            }
        }

        public void Stop()
        {
            _stream?.Close();
            _client?.Close();
            _listener?.Stop();
            IsConnected = false;
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
                //Header Length(4byte)
                byte[] headerSizeBuffer = new byte[4];
                await _stream.ReadExactlyAsync(headerSizeBuffer, 0, 4);
                int headerSize = BitConverter.ToInt32(headerSizeBuffer, 0);

                //Header Information (utf-8)
                byte[] headerBuffer = new byte[headerSize];
                await _stream.ReadExactlyAsync(headerBuffer, 0, headerSize);
                string header = System.Text.Encoding.UTF8.GetString(headerBuffer);
                string[] parts = header.Split('|');

                //Receive Image
                int imageSize = int.Parse(parts[3]);
                byte[] imageBuffer = new byte[imageSize];
                int totalSize = 0;
                while (totalSize < imageSize)
                {
                    int read = await _stream.ReadAsync(imageBuffer, totalSize, imageSize - totalSize);
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
                var model = new FileModel
                {
                    Message = MZEnum.GetDescription(BaseRole.Fail),
                };

                return model;
            }
        }
    }
}
