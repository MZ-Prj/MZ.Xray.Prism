using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace MZ.Util
{
    /// <summary>
    /// Undo/Redo(실행취소/다시실행) 기능 제공
    /// </summary>
    public class UndoRedoManager<T>
    {
        /// <summary>
        /// Undo
        /// </summary>
        private readonly Stack<List<T>> _undoStack = new();

        /// <summary>
        /// Redo
        /// </summary>
        private readonly Stack<List<T>> _redoStack = new();

        /// <summary>
        /// 상태 복사를 위한 복제 함수
        /// </summary>
        private readonly Func<T, T> _cloneFunc;

        public UndoRedoManager(Func<T, T> cloneFunc)
        {
            _cloneFunc = cloneFunc;
        }

        /// <summary>
        /// 실행취소(Undo) 가능한지 여부
        /// </summary>
        public bool CanUndo => _undoStack.Count > 1;
        /// <summary>
        /// 다시실행(Redo) 가능한지 여부
        /// </summary>
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// 현재 상태를 Undo 스택에 저장, Redo 스택 초기화
        /// </summary>
        public void SaveState(IEnumerable<T> currentState)
        {
            _undoStack.Push([.. currentState.Select(_cloneFunc)]);
            _redoStack.Clear();
        }

        /// <summary>
        /// Undo(이전 상태로 되돌리기), 현재 상태는 Redo 스택에 저장
        /// </summary>
        public List<T>? Undo(IEnumerable<T> currentState)
        {
            if (!CanUndo)
            {
                return null;
            }
            _redoStack.Push([.. currentState.Select(_cloneFunc)]);
            return _undoStack.Pop();
        }

        /// <summary>
        /// Redo(되돌리기 취소), 현재 상태는 Undo 스택에 저장
        /// </summary>
        public List<T>? Redo(IEnumerable<T> currentState)
        {
            if (!CanRedo)
            {
                return null;
            }
            _undoStack.Push([.. currentState.Select(_cloneFunc)]);
            return _redoStack.Pop();
        }

        /// <summary>
        /// 모든 상태 기록 초기화
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
