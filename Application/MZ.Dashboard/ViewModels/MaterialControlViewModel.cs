using MahApps.Metro.IconPacks;
using MZ.Core;
using MZ.Domain.Models;
using MZ.Loading;
using MZ.Util;
using MZ.Vision;
using MZ.Xray.Engine;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace MZ.Dashboard.ViewModels
{
    public class MaterialControlViewModel : MZBindableBase
    {
        #region Service
        private readonly IXrayService _xrayService;
        private readonly ILoadingService _loadingService;
        #endregion

        #region Wrapper
        public MaterialProcesser Material
        {
            get => _xrayService.Material;
            set => _xrayService.Material = value;
        }

        public ObservableCollection<MaterialControlModel> Controls
        {
            get => _xrayService.Material.Controls;
            set => _xrayService.Material.Controls = value;
        }

        #endregion

        #region Params

        private ObservableCollection<MaterialControlModel> _setControls = [];
        public ObservableCollection<MaterialControlModel> SetControls { get => _setControls; set => SetProperty(ref _setControls, value); }

        public ObservableCollection<IconButtonModel> WindowCommandButtons { get; } = [];

        #endregion

        #region Command
        private DelegateCommand<MaterialControlModel> _addCommand;
        public ICommand AddCommand => _addCommand ??= new DelegateCommand<MaterialControlModel>(MZAction.Wrapper<MaterialControlModel>(AddButton));

        private DelegateCommand<MaterialControlModel> _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new DelegateCommand<MaterialControlModel>(MZAction.Wrapper<MaterialControlModel>(DeleteButton));

        private DelegateCommand _undoCommand;
        public ICommand UndoCommand => _undoCommand ??= new DelegateCommand(MZAction.Wrapper(UndoButton));

        private DelegateCommand _redoCommand;
        public ICommand RedoCommand => _redoCommand ??= new DelegateCommand(MZAction.Wrapper(RedoButton));

        private DelegateCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new DelegateCommand(MZAction.Wrapper(RefreshButton));

        private DelegateCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new DelegateCommand(MZAction.Wrapper(SaveButton));

        #endregion

        public MaterialControlViewModel(IContainerExtension container, IXrayService xrayService, ILoadingService loadingService) : base(container)
        {
            _xrayService = xrayService;
            _loadingService = loadingService;

            base.Initialize();
        }

        public override void InitializeModel()
        {
            SetControls.Add(new(Material.UpdateAllMaterialGraph));

            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Undo), UndoCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Redo), RedoCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Refresh), RefreshCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.File), SaveCommand));


        }

        private void AddButton(MaterialControlModel model)
        {
            Controls.Add(new(Material.UpdateAllMaterialGraph)
            {
                Y=model.Y,
                XMin=model.XMin,
                XMax=model.XMax,
                Scalar=model.Scalar,
                Color=model.Color,
            });
            Material.UpdateAllMaterialGraph();

        }

        private void DeleteButton(MaterialControlModel model)
        {
            Controls.Remove(model);
            Material.UpdateAllMaterialGraph();

        }

        private void UndoButton()
        {
            Material.UpdateAllMaterialGraph();
        }

        private void RedoButton()
        {
            Material.UpdateAllMaterialGraph();
        }

        private void RefreshButton()
        {
            Material.UpdateAllMaterialGraph();
        }

        private void SaveButton()
        {
        }
    }
}
