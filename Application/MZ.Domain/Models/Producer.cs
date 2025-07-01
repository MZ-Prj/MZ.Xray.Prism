using OpenCvSharp;
using Prism.Mvvm;
using System;

namespace MZ.Domain.Models
{
    public class IpNetworkModel : BindableBase
    {
        private string _ip = "127.0.0.1";
        public string Ip { get => _ip; set => SetProperty(ref _ip, value); }

        private int _port = 5887;
        public int Port { get => _port; set => SetProperty(ref _port, value); }

        private bool _isConnected = false;
        public bool IsConnected { get => _isConnected; set => SetProperty(ref _isConnected, value); }

    }

    public class FileModel : BindableBase
    {
        private int _index;
        public int Index { get => _index; set => SetProperty(ref _index, value); }

        private string _path;
        public string Path { get => _path; set => SetProperty(ref _path, value); }

        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private Mat _image;
        public Mat Image { get => _image; set => SetProperty(ref _image, value); }

        private MatType _type;
        public MatType Type { get => _type; set => SetProperty(ref _type, value); }

        private int _width;
        public int Width { get => _width; set => SetProperty(ref _width, value); }

        private int _height;
        public int Height { get => _height; set => SetProperty(ref _height, value); }

        private string _message = string.Empty;
        public string Message { get => _message; set => SetProperty(ref _message, value); }

    }

}
