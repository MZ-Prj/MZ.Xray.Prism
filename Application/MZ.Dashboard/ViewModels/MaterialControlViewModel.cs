using MahApps.Metro.IconPacks;
using MZ.Core;
using MZ.Domain.Models;
using MZ.Loading;
using MZ.Util;
using MZ.Xray.Engine;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MZ.Dashboard.ViewModels
{
    public class MaterialControlViewModel : MZBindableBase
    {
        #region Services
        private readonly IXrayService _xrayService;
        private readonly ILoadingService _loadingService;
        #endregion

        #region Manager
        private readonly UndoRedoManager<MaterialControlModel> _undoRedoManager;
        #endregion

        #region Wrapper

        public MaterialProcesser Material
        {
            get => _xrayService.Material;
            set => _xrayService.Material = value;
        }

        #endregion

        #region Params

        private ObservableCollection<MaterialControlModel> _controls = [];
        public ObservableCollection<MaterialControlModel> Controls { get => _controls; set => SetProperty(ref _controls, value); }

        private ObservableCollection<MaterialControlModel> _setControls = [];
        public ObservableCollection<MaterialControlModel> SetControls { get => _setControls; set => SetProperty(ref _setControls, value); }

        public ObservableCollection<IconButtonModel> WindowCommandButtons { get; } = [];

        #endregion

        #region Commands

        private DelegateCommand<MaterialControlModel> _addCommand;
        public ICommand AddCommand => _addCommand ??= new DelegateCommand<MaterialControlModel>(MZAction.Wrapper<MaterialControlModel>(AddButton));

        private DelegateCommand<MaterialControlModel> _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new DelegateCommand<MaterialControlModel>(MZAction.Wrapper<MaterialControlModel>(DeleteButton));

        private DelegateCommand _undoCommand;
        public ICommand UndoCommand => _undoCommand ??= new DelegateCommand(MZAction.Wrapper(UndoButton), ()=>_undoRedoManager.CanUndo);

        private DelegateCommand _redoCommand;
        public ICommand RedoCommand => _redoCommand ??= new DelegateCommand(MZAction.Wrapper(RedoButton), ()=>_undoRedoManager.CanRedo);

        private DelegateCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new DelegateCommand(MZAction.Wrapper(RefreshButton));

        #endregion

        public MaterialControlViewModel(IContainerExtension container, IXrayService xrayService, ILoadingService loadingService)
            : base(container)
        {
            _xrayService = xrayService;
            _loadingService = loadingService;

            _undoRedoManager = new UndoRedoManager<MaterialControlModel>(model =>
                new MaterialControlModel(Material.UpdateAllMaterialGraph)
                {
                    Y = model.Y,
                    XMin = model.XMin,
                    XMax = model.XMax,
                    Scalar = model.Scalar,
                    Color = model.Color
                });

            base.Initialize();
        }

        public override void InitializeModel()
        {
            Controls = Material.Controls ?? [];
            SetControls.Add(new(Material.UpdateAllMaterialGraph));

            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Undo), UndoCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Redo), RedoCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Refresh), RefreshCommand));

            _undoRedoManager.SaveState(Controls);
        }

        private void AddButton(MaterialControlModel model)
        {
            _undoRedoManager.SaveState(Controls);

            Controls.Add(new MaterialControlModel(Material.UpdateAllMaterialGraph)
            {
                Y = model.Y,
                XMin = model.XMin,
                XMax = model.XMax,
                Scalar = model.Scalar,
                Color = model.Color
            });

            CopyControlsToMaterial();
            UpdateCanUndoRedo();

            Material.UpdateAllMaterialGraph();
        }

        private void DeleteButton(MaterialControlModel model)
        {
            _undoRedoManager.SaveState(Controls);

            Controls.Remove(model);

            CopyControlsToMaterial();
            UpdateCanUndoRedo();

            Material.UpdateAllMaterialGraph();
        }

        private void UndoButton()
        {
            var state = _undoRedoManager.Undo(Controls);
            if (state != null)
            {
                Controls = [.. state.Select(model => new MaterialControlModel(Material.UpdateAllMaterialGraph)
                    {
                        Y = model.Y,
                        XMin = model.XMin,
                        XMax = model.XMax,
                        Scalar = model.Scalar,
                        Color = model.Color
                    })];

                CopyControlsToMaterial();
                UpdateCanUndoRedo();

                Material.UpdateAllMaterialGraph();
            }
        }

        private void RedoButton()
        {
            var state = _undoRedoManager.Redo(Controls);
            if (state != null)
            {
                Controls = [.. state.Select(model => new MaterialControlModel(Material.UpdateAllMaterialGraph)
                    {
                        Y = model.Y,
                        XMin = model.XMin,
                        XMax = model.XMax,
                        Scalar = model.Scalar,
                        Color = model.Color
                    })];

                CopyControlsToMaterial();
                UpdateCanUndoRedo();

                Material.UpdateAllMaterialGraph();
            }
        }

        private void RefreshButton()
        {
            Material.UpdateAllMaterialGraph();
        }

        private void CopyControlsToMaterial()
        {
            Material.Controls = Controls;
        }

        private void UpdateCanUndoRedo()
        {
            _undoCommand.RaiseCanExecuteChanged();
            _redoCommand.RaiseCanExecuteChanged();
        }
    }

}
