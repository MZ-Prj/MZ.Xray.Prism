using System.Net.Sockets;
using System.Net;
using System.Text;
using MZ.Domain.Models;
using OpenCvSharp;

namespace MZ.ReceiveTest
{

    public class SocketReceiver
    {
        private TcpListener _listener;
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _isConnected;
        public bool IsConnected;

        public async Task StartListening(int port)
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, port);
                _listener.Start();
                Console.WriteLine($"Port {port}...");

                _client = await _listener.AcceptTcpClientAsync();
                _stream = _client.GetStream();
                IsConnected = true;
                Console.WriteLine("Connect!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void StopListening()
        {
            _stream?.Close();
            _client?.Close();
            _listener?.Stop();
            IsConnected = false;
            Console.WriteLine("Stopped");
        }

        public async Task<FileModel> ReceiveFileAsync()
        {
            var model = new FileModel();

            try
            {
                byte[] lengthBuffer = new byte[4];
                await _stream.ReadAsync(lengthBuffer, 0, 4);
                int infoLength = BitConverter.ToInt32(lengthBuffer, 0);

                byte[] infoBuffer = new byte[infoLength];
                await _stream.ReadAsync(infoBuffer, 0, infoLength);
                string info = Encoding.UTF8.GetString(infoBuffer);

                var parts = info.Split('|');
                model.Name = parts[0];
                model.Width = int.Parse(parts[1]);
                model.Height = int.Parse(parts[2]);
                int imageSize = int.Parse(parts[3]);

                byte[] imageBytes = new byte[imageSize];
                int totalRead = 0;
                while (totalRead < imageSize)
                {
                    int read = await _stream.ReadAsync(imageBytes, totalRead, imageSize - totalRead);
                    totalRead += read;
                }

                model.Image = Cv2.ImDecode(imageBytes, ImreadModes.Unchanged);
                model.Message = "Image Received successfully";

                return model;
            }
            catch (Exception ex)
            {
                model.Message = $"Error : {ex.Message}";
                return model;
            }
        }

        public void SaveImage(FileModel model)
        {
            try
            {
                if (model.Image == null || model.Image.Empty())
                {
                    model.Message = "No image data to save";
                    return;
                }

                string outputPath = Path.Combine(Environment.CurrentDirectory, "ReceivedFiles");
                Directory.CreateDirectory(outputPath);

                string fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{model.Name}.tiff";
                string fullPath = Path.Combine(outputPath, fileName);

                //model.Image.SaveImage(fullPath);
                model.Path = fullPath;
                model.Message = $"Path : {fullPath}";

                Console.WriteLine(model.Message);
            }
            catch (Exception ex)
            {
                model.Message = $"Error : {ex.Message}";
                Console.WriteLine(model.Message);
            }
        }
    }

}
