using MZ.Util;
using Prism.Mvvm;
using System;

namespace MZ.Domain.Models
{
    /// <summary>
    /// 비동기 작업, 네트워크, 데이터 처리 등에서 전체/영역별 Loading 상태 및 메시지 바인딩용 ViewModel
    /// - 로딩 진행중 -> 완료 상태 변경 및 메시지 갱신
    /// </summary>
    public class LoadingModel : BindableBase
    {
        private const string _defaultMessage = "Loading...";

        /// <summary>
        /// 로딩 상태(진행중/완료)
        /// - true: 로딩 진행중  
        /// - false: 로딩 아님(프로세스 종료)
        /// </summary>
        private bool _isLoading = false;
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

        /// <summary>
        /// 현재 표시중인 메시지  
        /// - 네트워크, 파일, DB 등 진행상황에 따라 갱신
        /// </summary>
        private string _message = "Loading...";
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        /// <summary>
        /// 로딩 시작 시 호출  
        /// - 메시지 갱신 후, IDisposable 반환(Dispose시 IsLoading 자동 해제)
        /// - using문 사용시, 종료 후 로딩 Off 처리 보장
        /// </summary>
        /// <param name="message">message : 로딩 중 표시할 메시지(생략시 기본 메시지)</param>
        /// <returns>IDisposable</returns>
        public IDisposable Show(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Message = message;
            }
            else
            {
                Message = _defaultMessage;
            }

            return MZDisposable.Wrapper((isLoading) =>
            {
                IsLoading = isLoading;
            });
        }
    }
}
