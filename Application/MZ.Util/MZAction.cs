using System;
using System.Diagnostics;
using MZ.Logger;

namespace MZ.Util
{
    public static class MZAction
    {
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

        public static Action Wrapper(Action action)
        {
            return () => Execute(action);
        }

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

        public static Action<T> Wrapper<T>(Action<T> action)
        {
            return (param) => Execute(action, param);
        }

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

        private static string GetClassName(Type type)
        {
            var className = type != null ? type.Name : "Unknown";
            return className;
        }
    }
}
