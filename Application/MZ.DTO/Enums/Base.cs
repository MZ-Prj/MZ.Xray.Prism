using System.ComponentModel.DataAnnotations;

namespace MZ.DTO.Enums
{
    /// <summary>
    /// 서비스 등에서 공통적으로 사용되는 기본 응답 결과 Enum
    /// 
    /// - 주요 상태/분기(경고, 성공, 실패, 유효성 문제)를 일관된 코드로 관리
    /// </summary>
    public enum BaseRole
    {
        /// <summary>
        /// 경고 (Warning)
        /// 
        /// - 일반적인 오류는 아니나 사용자나 시스템에 경고를 줘야 하는 상황
        /// - 예: 일부 입력값이 예상 범위를 벗어남, 처리에 지장은 없음
        /// - int 값: 199
        /// </summary>
        [Display(Name = "Warning", Description = "Warning")]
        Warning = 199,
        /// <summary>
        /// 성공 (Success)
        /// 
        /// - 요청이나 처리 작업이 정상적으로 완료된 경우
        /// - 예: 데이터 저장/조회 성공, 정상 응답
        /// - int 값: 200
        /// </summary>
        [Display(Name = "Success", Description = "Success")]
        Success = 200,
        /// <summary>
        /// 실패 (Fail)
        /// 
        /// - 요청이나 처리 작업이 비정상적으로 종료된 경우
        /// - 예: 저장/수정 실패, 예외 발생 등
        /// - int 값: 400
        /// </summary>
        [Display(Name = "Fail", Description = "Fail")]
        Fail = 400,
        /// <summary>
        /// 유효성 문제 (Valid)
        /// 
        /// - 데이터, 입력값 등에서 유효성 검증에 실패한 경우
        /// - 예: 필수값 누락, 형식 오류 등
        /// - int 값: 500
        /// </summary>
        [Display(Name = "Valid", Description = "Valid")]
        Valid = 500,
    }
}
