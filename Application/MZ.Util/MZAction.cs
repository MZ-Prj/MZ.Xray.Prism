using System;
using System.Diagnostics;
using MZ.Logger;

namespace MZ.Util
{
    /// <summary>
    /// Action/Func 기능 제공
    /// </summary>
    public static class MZAction
    {
        /// <summary>
        /// 전달받은 Func을 실행하며 예외 발생 시 로깅 후 기본값 반환
        /// </summary>
        public static T TryException<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                MZLogger.Error($"TryException : {func.Method.Name}\nMessage : {ex}");
                return default;
            }
        }

        /// <summary>
        /// 전달받은 Action을 실행하며, 실행 시간(ms) 측정 및 예외 로깅
        /// </summary>
        public static void TimerExecute(Action action)
        {
            string className = GetClassName(action.Method.DeclaringType);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                action();
            }
            catch (Exception ex)
            {
                MZLogger.Error($"Executing : {className}.{action.Method.Name}\nMessage : {ex}");
            }
            finally
            {
                stopwatch.Stop();
                MZLogger.Information($"Executing : {className}.{action.Method.Name} in {stopwatch.ElapsedMilliseconds} ms");
            }
        }

        /// <summary>
        /// Action을 예외처리 래퍼로 감싸는 함수
        /// </summary>
        public static Action Wrapper(Action action)
        {
            return () => Execute(action);
        }

        /// <summary>
        /// Action을 실행하며 예외 발생 시 로깅
        /// </summary>
        public static void Execute(Action action)
        {
            string className = GetClassName(action.Method.DeclaringType);
            MZLogger.Information($"Executing : {className}.{action.Method.Name}");
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MZLogger.Error($"Executing : {className}.{action.Method.Name}\nMessage : {ex}");
            }
        }

        /// <summary>
        /// Action을 예외처리 래퍼로 감싸는 함수
        /// </summary>
        public static Action<T> Wrapper<T>(Action<T> action)
        {
            return (param) => Execute(action, param);
        }

        /// <summary>
        /// Action 실행 및 로깅
        /// </summary>
        public static void Execute<T>(Action<T> action, T param)
        {
            string className = GetClassName(action.Method.DeclaringType);
            MZLogger.Information($"Executing : {className}.{action.Method.Name} with parameter : {param}");
            try
            {
                action(param);
            }
            catch (Exception ex)
            {
                MZLogger.Error($"Executing : {className}.{action.Method.Name}\nMessage : {ex}");
            }
        }

        /// <summary>
        /// 타입에서 클래스명 추출
        /// </summary>
        private static string GetClassName(Type type)
        {
            var className = type != null ? type.Name : "Unknown";
            return className;
        }
    }
}
