using OpenCvSharp;
using Prism.Mvvm;

namespace MZ.Model
{
    /// <summary>
    /// IP 네트워크(소켓/TCP 등) 연결 정보 바인딩용 모델
    /// 
    /// - 네트워크 장비/서버 통신 세팅 및 상태 관리  
    /// </summary>
    public class IpNetworkModel : BindableBase
    {
        /// <summary>
        /// 타겟 IP 주소 (기본: 127.0.0.1)
        /// </summary>
        private string _ip = "127.0.0.1";
        public string Ip { get => _ip; set => SetProperty(ref _ip, value); }

        /// <summary>
        /// 포트 번호 (기본: 5887)
        /// </summary>
        private int _port = 5887;
        public int Port { get => _port; set => SetProperty(ref _port, value); }

        /// <summary>
        /// 현재 연결 상태
        /// - true: 연결됨
        /// - false: 미연결
        /// </summary>
        private bool _isConnected = false;
        public bool IsConnected { get => _isConnected; set => SetProperty(ref _isConnected, value); }
    }

    /// <summary>
    /// 파일 및 이미지 데이터 바인딩용 모델 클래스
    /// 
    /// - UI/업무에서 파일 리스트, 이미지 프리뷰, 정보 표시 등에서 사용
    /// </summary>
    public class FileModel : BindableBase
    {
        /// <summary>
        /// 파일 순번
        /// </summary>
        private int _index;
        public int Index { get => _index; set => SetProperty(ref _index, value); }

        /// <summary>
        /// 파일 전체 경로(FullPath)
        /// </summary>
        private string _path;
        public string Path { get => _path; set => SetProperty(ref _path, value); }

        /// <summary>
        /// 파일명
        /// </summary>
        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        /// <summary>
        /// 이미지 데이터
        /// </summary>
        private Mat _image;
        public Mat Image { get => _image; set => SetProperty(ref _image, value); }

        /// <summary>
        /// 이미지 타입
        /// - 예: 16UC1, 8UC4
        /// </summary>
        private MatType _type;
        public MatType Type { get => _type; set => SetProperty(ref _type, value); }

        /// <summary>
        /// 가로
        /// </summary>
        private int _width;
        public int Width { get => _width; set => SetProperty(ref _width, value); }

        /// <summary>
        /// 세로
        /// </summary>
        private int _height;
        public int Height { get => _height; set => SetProperty(ref _height, value); }

        /// <summary>
        /// 전송여부와 관련된 메시지(성공, 실페)
        /// </summary>
        private string _message = string.Empty;
        public string Message { get => _message; set => SetProperty(ref _message, value); }

    }
}
