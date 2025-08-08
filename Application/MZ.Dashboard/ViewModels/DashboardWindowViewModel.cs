using MZ.Core;
using MZ.Loading;
using MZ.Util;
using MZ.Resource;
using MZ.Infrastructure;
using MZ.Auth.Views;
using MZ.Model;
using MZ.WindowDialog;
using MZ.Dashboard.Views;
using MZ.Xray.Engine;
using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Prism.Ioc;
using Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;
using static MZ.Core.MZModel;
using static MZ.Event.MZEvent;

namespace MZ.Dashboard.ViewModels
{
    /// <summary>
    /// Dashboard Window ViewModel :  각종 커맨드, 하위 컨트롤 관리
    /// </summary>
    public class DashboardWindowViewModel : MZBindableBase
    {
        #region Services
        private readonly ILoadingService _loadingService;
        private readonly IDatabaseService _databaseService;
        private readonly IXrayService _xrayService;
        private readonly IWindowDialogService _windowDialogService;
        #endregion

        #region Params
        private LoadingModel _loadingModel;
        public LoadingModel LoadingModel { get => _loadingModel ??= _loadingService[MZRegionNames.DashboardRegion]; set => SetProperty(ref _loadingModel, value); }

        private ObservableCollection<IconButtonModel> _windowCommandButtons = [];
        public ObservableCollection<IconButtonModel> WindowCommandButtons { get => _windowCommandButtons; set => SetProperty(ref _windowCommandButtons, value); }
        #endregion

        #region Commands

        private DelegateCommand _userInformationCommand;
        public ICommand UserInformationCommand => _userInformationCommand ??= new DelegateCommand(MZAction.Wrapper(UserInformationButton));

        private DelegateCommand _imageStorageCommand;
        public ICommand ImageStorageCommand => _imageStorageCommand ??= new DelegateCommand(MZAction.Wrapper(ImageStorageButton));

        private DelegateCommand _logStorageCommand;
        public ICommand LogStorageCommand => _logStorageCommand ??= new DelegateCommand(MZAction.Wrapper(LogStorageButton));

        private DelegateCommand _reportCommand;
        public ICommand ReportCommand => _reportCommand ??= new DelegateCommand(MZAction.Wrapper(ReportButton));

        private DelegateCommand _materialCommand;
        public ICommand MaterialCommand => _materialCommand ??= new DelegateCommand(MZAction.Wrapper(MaterialButton));

        private DelegateCommand _curveCommand;
        public ICommand CurveCommand => _curveCommand ??= new DelegateCommand(MZAction.Wrapper(CurveButton));

        private DelegateCommand _aiCommand;
        public ICommand AICommand => _aiCommand ??= new DelegateCommand(MZAction.Wrapper(AIButton));

        private DelegateCommand _themeCommand;
        public ICommand ThemeCommand => _themeCommand ??= new(MZAction.Wrapper(ThemeButton));

        private DelegateCommand _languageCommand;
        public ICommand LanguageCommand => _languageCommand ??= new(MZAction.Wrapper(LanguageButton));

        private DelegateCommand _logoutCommand;
        public ICommand LogoutCommand => _logoutCommand ??= new DelegateCommand(MZAction.Wrapper(LogoutButton));

        private DelegateCommand _windowClosingCommand;
        public ICommand WindowClosingCommand => _windowClosingCommand ??= new DelegateCommand(WindowClosing);

        #endregion

        public DashboardWindowViewModel(IContainerExtension container, IDatabaseService databaseService, ILoadingService loadingService, IWindowDialogService windowDialogService, IXrayService xrayService) : base(container)
        {
            _databaseService = databaseService;
            _loadingService = loadingService;
            _windowDialogService = windowDialogService;
            _xrayService = xrayService;

            base.Initialize();
        }

        public override void InitializeModel()
        {
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.FormatColorFill), MaterialCommand, isVisibility: false, tooltipKey: MZRegionNames.AddLng(MZRegionNames.MaterialControl)));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.ChartHistogram), CurveCommand, isVisibility: false, tooltipKey: MZRegionNames.AddLng(MZRegionNames.CurveControl)));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Robot), AICommand, isVisibility: false, tooltipKey: MZRegionNames.AddLng(MZRegionNames.AIControl)));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.FilePdfBox), ReportCommand, isVisibility: false, tooltipKey: MZRegionNames.AddLng(MZRegionNames.ReportControl)));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.ImageSearch), ImageStorageCommand, isVisibility: false, tooltipKey: MZRegionNames.AddLng(MZRegionNames.ImageStorageControl)));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.FileSearch), LogStorageCommand, isVisibility: false, tooltipKey: MZRegionNames.AddLng(MZRegionNames.LogStorageControl)));

            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Earth), LanguageCommand, tooltipKey: MZRegionNames.AddLng(MZRegionNames.LanguageRegion)));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.ThemeLightDark), ThemeCommand, tooltipKey: MZRegionNames.AddLng(MZRegionNames.ThemeRegion)));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Account), UserInformationCommand, isVisibility: false, tooltipKey: MZRegionNames.AddLng(MZRegionNames.UserInformationRegion)));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Logout), LogoutCommand, isVisibility: false, tooltipKey: MZRegionNames.AddLng(MZRegionNames.UserLogout)));
        }

        public override void InitializeEvent()
        {
            _eventAggregator.GetEvent<DashboardNavigationEvent>().Subscribe((NavigationModel model) =>
            {
                _regionManager.RequestNavigate(model.Region, model.View);
                UpdateWindowCommandButton(model.View == MZViewNames.DashboardControlView);
                LoadDatabase(model.View == MZViewNames.DashboardControlView);
            }, ThreadOption.UIThread, true);
        }

        /// <summary>
        /// Window가 닫힐시 데이터 저장 및 Application 종료
        /// </summary>
        private void WindowClosing()
        {
            SaveDatabase();

            Application.Current.Shutdown();
        }

        /// <summary>
        /// 사용자 정보
        /// </summary>
        private void UserInformationButton()
        {
            _dialogService.ShowDialog(
                "DialogView",
                new DialogParameters
                {
                    {"Title",  LanguageService.GetString(MZRegionNames.AddLng(MZRegionNames.UserInformationRegion)) },
                    {"RegionName", MZRegionNames.UserInformationRegion}
                },
                (IDialogResult result) => {
                });
        }

        /// <summary>
        /// 태마 변경
        /// </summary>
        private void ThemeButton()
        {
            _databaseService.User.ChangeTheme(new(ThemeService.ChangeMode()));
        }

        /// <summary>
        /// 언어 변경
        /// </summary>
        private void LanguageButton()
        {
            _dialogService.ShowDialog(
                "DialogView",
                new DialogParameters
                {
                    {"Title",  LanguageService.GetString(MZRegionNames.AddLng(MZRegionNames.LanguageRegion)) },
                    {"RegionName", MZRegionNames.LanguageRegion}
                },
                (IDialogResult result) => {
                    _databaseService.User.ChangeLanguage(new(LanguageService.GetCurrentLanguageRole()));
                });
        }

        /// <summary>
        /// 로그아웃 및 로그인 뷰 이벤트
        /// </summary>
        private void LogoutButton()
        {
            SaveDatabase();

            _eventAggregator.GetEvent<DashboardNavigationEvent>().Publish(
                        new NavigationModel(
                            MZRegionNames.DashboardRegion,
                            nameof(UserLoginView)));
        }

        /// <summary>
        /// 이미지 스토리지 창
        /// </summary>
        private async void ImageStorageButton()
        {
            await _windowDialogService.ShowWindow(
                title: LanguageService.GetString(MZRegionNames.AddLng(MZRegionNames.ImageStorageControl)),
                regionName: nameof(ImageStorageControlView),
                isMultiple: false);
        }

        /// <summary>
        /// 로그 스토리지 창
        /// </summary>
        private async void LogStorageButton()
        {
            await _windowDialogService.ShowWindow(
                title: LanguageService.GetString(MZRegionNames.AddLng(MZRegionNames.LogStorageControl)),
                regionName: nameof(LogStorageControlView),
                isMultiple:false);
        }

        /// <summary>
        /// 물성분석 제어 창
        /// </summary>
        private async void MaterialButton()
        {
            await _windowDialogService.ShowWindow(
                title: LanguageService.GetString(MZRegionNames.AddLng(MZRegionNames.MaterialControl)),
                regionName: nameof(MaterialControlView),
                isMultiple: false,
                resizeMode: ResizeMode.NoResize,
                width: 480,
                height: 640);
        }

        /// <summary>
        /// 인공지능 카테고리 제어 창
        /// </summary>
        private async void AIButton()
        {
            await _windowDialogService.ShowWindow(
                title: LanguageService.GetString(MZRegionNames.AddLng(MZRegionNames.AIControl)),
                regionName: nameof(AIControlView),
                isMultiple: false,
                resizeMode: ResizeMode.NoResize,
                width: 480,
                height: 640);
        }

        /// <summary>
        /// LUT 곡선 제어 창
        /// </summary>
        private async void CurveButton()
        {
            await _windowDialogService.ShowWindow(
                title: LanguageService.GetString(MZRegionNames.AddLng(MZRegionNames.CurveControl)),
                regionName: nameof(CurveControlView),
                isMultiple: false,
                resizeMode: ResizeMode.NoResize,
                width: 260,
                height: 520);
        }

        /// <summary>
        /// 분석 보고서 창 
        /// </summary>
        private async void ReportButton()
        {
            await _windowDialogService.ShowWindow(
                title: LanguageService.GetString(MZRegionNames.AddLng(MZRegionNames.ReportControl)),
                regionName: nameof(ReportControlView),
                isMultiple: false);
        }

        /// <summary>
        /// 커맨드 버튼 노출 여부
        /// </summary>
        /// <param name="check">bool</param>
        private async void UpdateWindowCommandButton(bool check)
        {
            var response = await _databaseService.User.IsAdmin();
            bool isAdmin = response.Data;

            foreach (var button in WindowCommandButtons)
            {
                button.IsVisibility = button.IconKind switch
                {
                    nameof(PackIconMaterialKind.FileSearch) => isAdmin,
                    nameof(PackIconMaterialKind.Earth) => true,
                    nameof(PackIconMaterialKind.ThemeLightDark) => true,
                    _ => check,
                };
            }

        }

        /// <summary>
        /// 데이터 베이스 처리
        /// </summary>
        private void SaveDatabase()
        {
            _xrayService.SaveDatabase();
        }

        /// <summary>
        /// 데이터 베이스 로딩 및 사용자 환경 설정 반영
        /// </summary>
        /// <param name="check">bool</param>
        private void LoadDatabase(bool check)
        {
            if (check)
            {
                _xrayService.LoadDatabase();
            }
        }

    }
}