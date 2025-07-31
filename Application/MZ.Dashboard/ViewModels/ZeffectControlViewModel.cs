using MahApps.Metro.IconPacks;
using MZ.Core;
using MZ.Domain.Models;
using MZ.Util;
using MZ.Xray.Engine;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MZ.Dashboard.ViewModels
{
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

            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Undo), UndoCommand, tooltip: "tooltip"));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Redo), RedoCommand, tooltip: "tooltip"));

            _undoRedoManager.SaveState(Controls);

        }

        private void AddButton(ZeffectControlModel model)
        {
            _undoRedoManager.SaveState(Controls);
        }

        private void DeleteButton(ZeffectControlModel model)
        {
            _undoRedoManager.SaveState(Controls);
        }

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

        private void CopyControlsToZeffect()
        {
            Zeffect.Controls = Controls;
        }

        private void UpdateCanUndoRedo()
        {
            _undoCommand.RaiseCanExecuteChanged();
            _redoCommand.RaiseCanExecuteChanged();
        }
    }
}
