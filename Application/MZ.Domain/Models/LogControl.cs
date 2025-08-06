using System;
using System.Windows.Media;

namespace MZ.Domain.Models
{
    /// <summary>
    /// 로그 레벨(Info/Warning/Error) 선택 콤보박스용 모델 클래스
    /// </summary>
    public class LogComboboxModel
    {
        /// <summary>
        /// 콤보박스에 표시될 로그 레벨("info", "warning", "error")
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 레벨별 구분 색상
        /// </summary>
        public SolidColorBrush ColorBrush { get; set; }
    }

    /// <summary>
    /// 단일 로그 라인(행) 정보 모델 클래스
    /// 
    /// - 로그 출력/검색/필터링 뷰(그리드/리스트 등) 바인딩에 사용
    /// - 각 로그의 발생 시간, 레벨, 메시지, 색상 등 표시
    /// </summary>
    public class LogControlModel
    {
        /// <summary>
        /// 로그 발생 시각(파일명/라인 등으로부터 파싱)
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 로그 레벨("info", "warning", "error")
        /// </summary>
        public string LogLevel { get; set; }
        /// <summary>
        /// 해당 파일 내 라인 번호
        /// </summary>
        public int LineNumber { get; set; }
        /// <summary>
        /// 로그 메시지
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 로그 레벨별 색상(경고/에러 등 시각화)
        /// </summary>
        public SolidColorBrush ColorBrush { get; set; }
    }
}
