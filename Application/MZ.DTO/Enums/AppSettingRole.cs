using System.ComponentModel.DataAnnotations;

namespace MZ.DTO.Enums
{
    /// <summary>
    /// 앱 설정 관련 응답 결과 Enum
    /// 
    /// - 클라이언트/서버 등에서 AppSetting 작업(저장, 조회 등) 후 상태를 표현
    /// - 코드값 기준으로 외부 응답/내부 분기 등에 사용
    /// </summary>
    public enum AppSettingRole
    {
        /// <summary>
        /// 성공
        /// </summary>
        [Display(Name = "Success", Description = "App Setting Success")]
        Success = 200,
        /// <summary>
        /// 실패
        /// </summary>
        [Display(Name = "Fail", Description = "App Setting Fail")]
        Fail = 400,
        /// <summary>
        /// 유효성 검증 실패
        /// </summary>
        [Display(Name = "Valid", Description = "App Setting Valid")]
        Valid = 500,
    }
}
