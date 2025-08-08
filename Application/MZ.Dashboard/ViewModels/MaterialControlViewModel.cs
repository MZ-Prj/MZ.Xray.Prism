using MahApps.Metro.IconPacks;
using MZ.Core;
using MZ.Model;
using MZ.Util;
using MZ.Xray.Engine;
using Prism.Commands;
using Prism.Ioc;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace MZ.Dashboard.ViewModels
{
    /// <summary>
    /// Material Control ViewModel : 물질분석(xray) 색상 표현 제어
    /// </summary>
    public class MaterialControlViewModel : MZBindableBase
    {
        #region Services
        private readonly IXrayService _xrayService;

        public MaterialProcesser Material
        {
            get => _xrayService.Material;
            set => _xrayService.Material = value;
        }
        #endregion

        #region Manager
        private readonly UndoRedoManager<MaterialControlModel> _undoRedoManager;
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

        public MaterialControlViewModel(IContainerExtension container, IXrayService xrayService)
            : base(container)
        {
            _xrayService = xrayService;

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

            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Undo), UndoCommand, tooltipKey: MZRegionNames.AddLng(MZRegionNames.CommonUndo)));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Redo), RedoCommand, tooltipKey: MZRegionNames.AddLng(MZRegionNames.CommonRedo)));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Refresh), RefreshCommand, tooltipKey: MZRegionNames.AddLng(MZRegionNames.CommonRefresh)));

            _undoRedoManager.SaveState(Controls);

        }

        /// <summary>
        /// 물성분석 색상 항목 추가
        /// </summary>
        /// <param name="model">MaterialControlModel : 추가 항목</param>
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


        /// <summary>
        /// 물성분석 색상 항목 삭제
        /// </summary>
        /// <param name="model">MaterialControlModel : 삭제 항목</param>
        private void DeleteButton(MaterialControlModel model)
        {
            _undoRedoManager.SaveState(Controls);

            Controls.Remove(model);

            CopyControlsToMaterial();
            UpdateCanUndoRedo();

            Material.UpdateAllMaterialGraph();
        }


        /// <summary>
        /// Undo 
        /// </summary>
        private void UndoButton()
        {
            var state = _undoRedoManager.Undo(Controls);
            if (state != null)
            {
                Controls = [.. state.Select(model => new MaterialControlModel(Material.UpdateAllMaterialGraph)
                    {
                        Id = model.Id,
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

        /// <summary>
        /// Redo 
        /// </summary>
        private void RedoButton()
        {
            var state = _undoRedoManager.Redo(Controls);
            if (state != null)
            {
                Controls = [.. state.Select(model => new MaterialControlModel(Material.UpdateAllMaterialGraph)
                    {
                        Id = model.Id,
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

        /// <summary>
        /// 물성분석 ColorMap 새로고침
        /// </summary>
        private void RefreshButton()
        {
            Material.UpdateAllMaterialGraph();
        }

        /// <summary>
        /// Controls -> Material.Controls 복사
        /// </summary>
        private void CopyControlsToMaterial()
        {
            Material.Controls = Controls;
        }
        /// <summary>
        /// Undo/Redo 가능 갱신
        /// </summary>
        private void UpdateCanUndoRedo()
        {
            _undoCommand.RaiseCanExecuteChanged();
            _redoCommand.RaiseCanExecuteChanged();
        }
    }
}
