using System;

namespace MZ.DTO
{
#nullable enable
    public record BaseResponse<TCode, TData>(
        bool Success,
        TCode? Code,
        TData? Data,
        Exception? Error
    );

    public static class BaseResponseExtensions
    {
        public static BaseResponse<TCode, TData> Success<TCode, TData>(TCode code, TData data)
            => new(true, code, data, null);
        public static BaseResponse<TCode, TData?> Success<TCode, TData>(TCode code)
            => new(true, code, default, null);
        public static BaseResponse<TCode, TData?> Failure<TCode, TData>(TCode code, Exception? ex = null)
            => new(false, code, default, ex);
    }
}
