using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace MZ.Util
{
    public class UndoRedoManager<T>
    {
        private readonly Stack<List<T>> _undoStack = new();
        private readonly Stack<List<T>> _redoStack = new();
        private readonly Func<T, T> _cloneFunc;

        public UndoRedoManager(Func<T, T> cloneFunc)
        {
            _cloneFunc = cloneFunc;
        }

        public bool CanUndo => _undoStack.Count > 1;
        public bool CanRedo => _redoStack.Count > 0;

        public void SaveState(IEnumerable<T> currentState)
        {
            _undoStack.Push([.. currentState.Select(_cloneFunc)]);
            _redoStack.Clear();
        }

        public List<T>? Undo(IEnumerable<T> currentState)
        {
            if (!CanUndo)
            {
                return null;
            }
            _redoStack.Push([.. currentState.Select(_cloneFunc)]);
            return _undoStack.Pop();
        }

        public List<T>? Redo(IEnumerable<T> currentState)
        {
            if (!CanRedo)
            {
                return null;
            }
            _undoStack.Push([.. currentState.Select(_cloneFunc)]);
            return _redoStack.Pop();
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
