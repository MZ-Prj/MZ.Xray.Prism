using MZ.Logger;
using System;

namespace MZ.Util
{
    public static class MZDisposable
    {
        private sealed class DisposableWrapper : IDisposable
        {
            private Action _disposeAction;
            public DisposableWrapper(Action disposeAction)
            {
                _disposeAction = disposeAction;
            }

            public void Dispose()
            {
                _disposeAction?.Invoke();
                _disposeAction = null;
            }
        }

        public static IDisposable Create(Action start, Action dispose)
        {
            start?.Invoke();
            return new DisposableWrapper(dispose);
        }

        public static IDisposable Wrapper(Action start, Action dispose)
        {
            string className = GetClassName(start.Method.DeclaringType);
            MZLogger.Information($"Executing : {className}.{start.Method.Name} ");

            return Create(start, () =>
            {
                dispose();
                string className = GetClassName(dispose.Method.DeclaringType);
                MZLogger.Information($"Executing : {className}.{dispose.Method.Name} ");
            });
        }

        public static IDisposable Wrapper(Action<bool> action)
        {
            string className = GetClassName(action.Method.DeclaringType);

            MZLogger.Information($"Executing : {className}.{action.Method.Name} ");

            action(true);
            return Create(null, () =>
            {
                action(false);
                MZLogger.Information($"Executing : {className}.{action.Method.Name} ");
            });
        }

        private static string GetClassName(Type type)
        {
            var className = type != null ? type.Name : "Unknown";
            return className;
        }
    }
}
