using MahApps.Metro.IconPacks;
using MZ.Core;
using MZ.Util;
using MZ.Xray.Engine;
using MZ.Model;
using Prism.Ioc;
using Prism.Commands;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace MZ.Dashboard.ViewModels
{
    /// <summary>
    /// Zeffect Control ViewModel : 물성 제어 
    /// </summary>
    public class ZeffectControlViewModel : MZBindableBase
    {
        #region Services
        private readonly IXrayService _xrayService;
        #endregion

        #region Manager
        private readonly UndoRedoManager<ZeffectControlModel> _undoRedoManager;
        #endregion

        #region Wrapper

        public ZeffectProcesser Zeffect
        {
            get => _xrayService.Zeffect;
            set => _xrayService.Zeffect = value;
        }

        #endregion

        #region Params

        private ObservableCollection<ZeffectControlModel> _controls = [];
        public ObservableCollection<ZeffectControlModel> Controls { get => _controls; set => SetProperty(ref _controls, value); }

        private ObservableCollection<ZeffectControlModel> _setControls = [];
        public ObservableCollection<ZeffectControlModel> SetControls { get => _setControls; set => SetProperty(ref _setControls, value); }

        public ObservableCollection<IconButtonModel> WindowCommandButtons { get; } = [];

        #endregion

        #region Commands

        private DelegateCommand<ZeffectControlModel> _addCommand;
        public ICommand AddCommand => _addCommand ??= new DelegateCommand<ZeffectControlModel>(MZAction.Wrapper<ZeffectControlModel>(AddButton));

        private DelegateCommand<ZeffectControlModel> _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new DelegateCommand<ZeffectControlModel>(MZAction.Wrapper<ZeffectControlModel>(DeleteButton));


        private DelegateCommand _undoCommand;
        public ICommand UndoCommand => _undoCommand ??= new DelegateCommand(MZAction.Wrapper(UndoButton), () => _undoRedoManager.CanUndo);

        private DelegateCommand _redoCommand;
        public ICommand RedoCommand => _redoCommand ??= new DelegateCommand(MZAction.Wrapper(RedoButton), () => _undoRedoManager.CanRedo);

        #endregion
        public ZeffectControlViewModel(IContainerExtension container, IXrayService xrayService) : base(container)
        {
            _xrayService = xrayService;

            _undoRedoManager = new UndoRedoManager<ZeffectControlModel>(model =>
                new ZeffectControlModel()
                {
                    Content = model.Content,
                    Check = model.Check,
                    Min = model.Min,
                    Max = model.Max,
                    Scalar = model.Scalar,
                    Color = model.Color
                });

            base.Initialize();

        }

        public override void InitializeModel()
        {
            Controls = Zeffect.Controls ?? [];
            SetControls.Add(new());

            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Undo), UndoCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Redo), RedoCommand));

            _undoRedoManager.SaveState(Controls);

        }

        /// <summary>
        /// Zeffect에서 볼 영역 추가
        /// </summary>
        /// <param name="model">ZeffectControlModel</param>
        private void AddButton(ZeffectControlModel model)
        {
            _undoRedoManager.SaveState(Controls);

            Controls.Add(new ZeffectControlModel()
            {
                Content = model.Content,
                Check = model.Check,
                Min = model.Min,
                Max = model.Max,
                Scalar = model.Scalar,
            });

            CopyControlsToZeffect();
            UpdateCanUndoRedo();
        }

        /// <summary>
        /// Zeffect에서 볼 영역 삭제
        /// </summary>
        /// <param name="model">ZeffectControlModel</param>
        private void DeleteButton(ZeffectControlModel model)
        {
            _undoRedoManager.SaveState(Controls);
            
            Controls.Remove(model);

            CopyControlsToZeffect();
            UpdateCanUndoRedo();
        }

        /// <summary>
        /// Undo
        /// </summary>
        private void UndoButton()
        {
            var state = _undoRedoManager.Undo(Controls);
            if (state != null)
            {
                Controls = [.. state.Select(model => new ZeffectControlModel()
                    {
                        Content = model.Content,
                        Check = model.Check,
                        Min = model.Min,
                        Max = model.Max,
                        Scalar = model.Scalar,
                        Color = model.Color
                    })];

                CopyControlsToZeffect();
                UpdateCanUndoRedo();
            }
        }

        /// <summary>
        /// Redo
        /// </summary>
        private void RedoButton()
        {

            var state = _undoRedoManager.Redo(Controls);
            if (state != null)
            {
                Controls = [.. state.Select(model => new ZeffectControlModel()
                    {
                        Content = model.Content,
                        Check = model.Check,
                        Min = model.Min,
                        Max = model.Max,
                        Scalar = model.Scalar,
                        Color = model.Color
                    })];

                CopyControlsToZeffect();
                UpdateCanUndoRedo();
            }
        }

        /// <summary>
        /// Controls -> Zeffect.Controls 복사
        /// </summary>
        private void CopyControlsToZeffect()
        {
            Zeffect.Controls = Controls;
        }

        /// <summary>
        /// Undo/Redo 갱신
        /// </summary>
        private void UpdateCanUndoRedo()
        {
            _undoCommand.RaiseCanExecuteChanged();
            _redoCommand.RaiseCanExecuteChanged();
        }
    }
}
