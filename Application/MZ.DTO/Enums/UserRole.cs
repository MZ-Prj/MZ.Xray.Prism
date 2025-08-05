using System.ComponentModel.DataAnnotations;

namespace MZ.DTO.Enums
{
    /// <summary>
    /// 사용자 로그인 결과 상태 Enum
    /// 
    /// - 로그인 시도에 대한 결과 상태값을 의미
    /// </summary>
    public enum UserLoginRole
    {
        /// <summary>
        /// 로그인 성공
        /// 
        /// - 정상적으로 아이디/비밀번호가 일치하여 로그인 성공
        /// - int 값: 200
        /// </summary>
        [Display(Name = "Success", Description = "User Login Success")]
        Success = 200,
        /// <summary>
        /// 로그인 실패
        /// 
        /// - 비밀번호가 일치하지 않거나 인증이 실패한 경우
        /// - int 값: 400
        /// </summary>
        [Display(Name = "Fail", Description = "User Login Fail")]
        Fail = 400,
        /// <summary>
        /// 사용자 정보 없음
        /// 
        /// - 해당 사용자 아이디(Username)가 DB에 존재하지 않을 때
        /// - int 값: 401
        /// </summary>
        [Display(Name = "NotFound", Description = "User Information NotFound")]
        NotFound = 401,
        /// <summary>
        /// 유효성 검증 실패
        /// 
        /// - 필수 입력값 누락, 형식 오류 등으로 유효성 검증이 통과되지 않을 때
        /// - int 값: 500
        /// </summary>
        [Display(Name = "Valid", Description = "User Login Valid")]
        Valid = 500,
    }
    /// <summary>
    /// 사용자 회원가입 결과 상태 Enum
    /// 
    /// - 회원가입 시도에 대한 결과 상태값을 의미
    /// </summary>
    public enum UserRegisterRole
    {
        /// <summary>
        /// 회원가입 성공
        /// 
        /// - 모든 검증 통과 및 정상적으로 가입 처리 완료
        /// - int 값: 200
        /// </summary>
        [Display(Name = "Success", Description = "User Register Success")]
        Success = 200,
        /// <summary>
        /// 회원가입 실패
        /// 
        /// - 시스템 에러, 저장 실패 등으로 가입이 정상적으로 처리되지 않은 경우
        /// - int 값: 400
        /// </summary>
        [Display(Name = "Fail", Description = "User Register Fail")]
        Fail = 400,
        /// <summary>
        /// 비밀번호 불일치
        /// 
        /// - 비밀번호/비밀번호 재입력 값이 서로 다를 때
        /// - int 값: 401
        /// </summary>
        [Display(Name = "NotMatch", Description = "Check Password & Repassword")]
        NotMatchPassword = 401,
        /// <summary>
        /// 이미 존재하는 사용자
        /// 
        /// - 입력한 아이디(Username)가 이미 등록되어 있을 때
        /// - int 값: 402
        /// </summary>
        [Display(Name = "AleadyExist", Description = "User Aleady Exist")]
        AleadyExist = 402,
        /// <summary>
        /// 유효성 검증 실패
        /// 
        /// - 필수 입력값 누락, 형식 오류 등으로 유효성 검증이 통과되지 않을 때
        /// - int 값: 500
        /// </summary>
        [Display(Name = "Valid", Description = "User Login Valid")]
        Valid = 500,
    }
}
