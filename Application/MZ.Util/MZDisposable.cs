using MZ.Logger;
using System;
using System.Collections.ObjectModel;

namespace MZ.Util
{
    /// <summary>
    /// 리소스 해제 및 래핑 기능을 제공하는 유틸리티 클래스
    /// </summary>
    public static class MZDisposable
    {
        /// <summary>
        /// 전달된 Action을 IDisposable로 래핑
        /// </summary>
        private class DisposableWrapper : IDisposable
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

        /// <summary>
        /// 시작/해제 래핑 객체를 생성함
        /// </summary>
        public static IDisposable Create(Action start, Action dispose)
        {
            start?.Invoke();
            return new DisposableWrapper(dispose);
        }

        /// <summary>
        /// 시작/해제 래핑
        /// </summary>
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

        /// <summary>
        /// bool형 파라미터로 받는 액션을 사용해 시작/해제
        /// </summary>
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

        /// <summary>
        /// ObservableCollection의 모든 요소를 Dispose하고 컬렉션을 비움
        /// </summary>
        public static void DisposeAndClear<T>(this ObservableCollection<T> collection)
        {
            if (collection == null) return;

            foreach (var item in collection)
            {
                if (item is IDisposable disposable)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        MZLogger.Error(ex.ToString());
                    }
                }
            }
            collection.Clear();
        }

        /// <summary>
        /// 클래스 이름 반환
        /// </summary>
        private static string GetClassName(Type type)
        {
            var className = type != null ? type.Name : "Unknown";
            return className;
        }
    }
}
