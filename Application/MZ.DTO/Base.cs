using System;

#nullable enable
namespace MZ.DTO
{
    /// <summary>
    /// 공통 응답 DTO
    /// 
    /// - 제네릭으로 코드와 데이터 타입을 지정
    /// </summary>
    /// <typeparam name="TCode">응답 코드(상태, 에러, 결과 분류 등)</typeparam>
    /// <typeparam name="TData">실제 반환 데이터(엔티티, 리스트, 값 등)</typeparam>
    /// <param name="Success">bool : 요청 처리 성공 여부</param>
    /// <param name="Code">응답 코드</param>
    /// <param name="Data">실제 반환 데이터</param>
    /// <param name="Error">실패 시 예외 정보</param>
    public record BaseResponse<TCode, TData>(
        bool Success,
        TCode? Code,
        TData? Data,
        Exception? Error
    );

    /// <summary>
    /// BaseResponse 생성 및 결과 처리 확장 메서드
    /// </summary>
    public static class BaseResponseExtensions
    {
        /// <summary>
        /// 성공 응답 생성 (데이터 포함)
        /// </summary>
        /// <typeparam name="TCode">응답 코드 타입</typeparam>
        /// <typeparam name="TData">반환 데이터 타입</typeparam>
        /// <param name="code">응답 코드</param>
        /// <param name="data">반환 데이터</param>
        /// <returns>성공 응답 객체</returns>
        public static BaseResponse<TCode, TData> Success<TCode, TData>(TCode code, TData data)
            => new(true, code, data, null);

        /// <summary>
        /// 성공 응답 생성 (데이터 없이 코드만)
        /// </summary>
        /// <typeparam name="TCode">응답 코드 타입</typeparam>
        /// <typeparam name="TData">반환 데이터 타입</typeparam>
        /// <param name="code">응답 코드</param>
        /// <returns>성공 응답 객체(optional)</returns>
        public static BaseResponse<TCode, TData?> Success<TCode, TData>(TCode code)
            => new(true, code, default, null);
        /// <summary>
        /// 실패 응답 생성 (에러, 데이터 없음)
        /// </summary>
        /// <typeparam name="TCode">응답 코드 타입</typeparam>
        /// <typeparam name="TData">반환 데이터 타입</typeparam>
        /// <param name="code">응답 코드</param>
        /// <param name="ex">예외 정보(선택)</param>
        /// <returns>실패 응답 객체</returns>
        public static BaseResponse<TCode, TData?> Failure<TCode, TData>(TCode code, Exception? ex = null)
            => new(false, code, default, ex);
    }
}
