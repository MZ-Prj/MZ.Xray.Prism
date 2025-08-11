using MZ.Resource;

namespace MZ.Core
{

    /// <summary>
    /// 다른 모듈에 있는 View Name을 불러오기 위함
    /// </summary>
    public static class MZViewNames
    {
        public readonly static string DashboardControlView = "DashboardControlView";
    }

    public static partial class MZRegionNames
    {
        private static string Key = "Lng";
        public static string AddLng(string resourceName)
        {
            return $"{Key}{resourceName}";
        }

        public static string GetLanguage(string name)
        {
            return LanguageService.GetString(AddLng(name));
        }
    }

    public static partial class MZRegionNames
    {
        //Common
        public readonly static string DashboardRegion = "DashboardRegion";
        public readonly static string SplashRegion = "SplashRegion";

        //Template
        public readonly static string DialogRegion = "DialogRegion";
        public readonly static string WindowDialogRegion = "WindowDialogRegion";
        public readonly static string LanguageRegion = "LanguageRegion";
        public readonly static string LoadingRegion = "LoadingRegion";
        public readonly static string ThemeRegion = "ThemeRegion";

        //UI
        public readonly static string SidebarRegion = "SidebarRegion";
        public readonly static string ToolbarRegion = "ToolbarRegion";

        //Application
        public readonly static string DashboardFooterButtonControlRegion = "DashboardFooterButtonControlRegion";
        public readonly static string LogStorageControl = "LogStorageControlRegion";
        public readonly static string ImageStorageControl = "ImageStorageControlRegion";
        public readonly static string MaterialControl = "MaterialControlRegion";
        public readonly static string AIControl = "AIControlRegion";
        public readonly static string ZeffectControl = "ZeffectControlRegion";
        public readonly static string CurveControl = "CurveControlRegion";
        public readonly static string ReportControl = "ReportControlRegion";
        
        //Producer
        public readonly static string IpNetworkRegion = "IpNetworkRegion";

        //User
        public readonly static string UserLoginRegion = "UserLoginRegion";
        public readonly static string UserRegisterRegion = "UserRegisterRegion";
        public readonly static string UserInformationRegion = "UserInformationRegion";
        public readonly static string UserLogout = "UserLogoutRegion";
    }

    public static partial class MZRegionNames
    {
        //Save (File Dialog)
        public readonly static string SavePDF = "Save_PDF";
        public readonly static string SaveScreen = "Save_Screen";

        //Common Button
        public readonly static string CommonRefresh = "Common_Refresh";
        public readonly static string CommonSearch = "Common_Search";
        public readonly static string CommonRedo = "Common_Redo";
        public readonly static string CommonUndo = "Common_Undo";

        //Footer Button (Resource)
        public readonly static string XrayRealtimeRegion = "XrayRealtimeRegion";
        public readonly static string XrayRealtimeRegionAIOnOff = "XrayRealtimeRegion_AIOnOff";
        public readonly static string XrayRealtimeRegionBrightDown = "XrayRealtimeRegion_BrightDown";
        public readonly static string XrayRealtimeRegionBrighUp = "XrayRealtimeRegion_BrighUp";
        public readonly static string XrayRealtimeRegionColor = "XrayRealtimeRegion_Color";
        public readonly static string XrayRealtimeRegionContrastDown = "XrayRealtimeRegion_ContrastDown";
        public readonly static string XrayRealtimeRegionContrastUp = "XrayRealtimeRegion_ContrastUp";
        public readonly static string XrayRealtimeRegionFilterClear = "XrayRealtimeRegion_FilterClear";
        public readonly static string XrayRealtimeRegionGray = "XrayRealtimeRegion_Gray";
        public readonly static string XrayRealtimeRegionInorganic = "XrayRealtimeRegion_Inorganic";
        public readonly static string XrayRealtimeRegionMetal = "XrayRealtimeRegion_Metal";
        public readonly static string XrayRealtimeRegionNext = "XrayRealtimeRegion_Next";
        public readonly static string XrayRealtimeRegionOrganic = "XrayRealtimeRegion_Organic";
        public readonly static string XrayRealtimeRegionPicker = "XrayRealtimeRegion_Picker";
        public readonly static string XrayRealtimeRegionPlayStop = "XrayRealtimeRegion_PlayStop";
        public readonly static string XrayRealtimeRegionPrevious = "XrayRealtimeRegion_Previous";
        public readonly static string XrayRealtimeRegionSaveImage = "XrayRealtimeRegion_SaveImage";
        public readonly static string XrayRealtimeRegionSetting = "XrayRealtimeRegion_Setting";
        public readonly static string XrayRealtimeRegionZeffect = "XrayRealtimeRegion_Zeffect";
        public readonly static string XrayRealtimeRegionZoomIn = "XrayRealtimeRegion_ZoomIn";
        public readonly static string XrayRealtimeRegionZoomOut = "XrayRealtimeRegion_ZoomOut";
        public readonly static string XrayRealtimeRegionRelativeWidthRatioDown = "XrayRealtimeRegion_RelativeWidthRatioDown";
        public readonly static string XrayRealtimeRegionRelativeWidthRatioUp = "XrayRealtimeRegion_RelativeWidthRatioUp";

        //Message (Loading, Splash)
        public readonly static string SplashRegionLoading = "SplashRegion_Loading";
        public readonly static string SplashRegionSuccess = "SplashRegion_Success";
        public readonly static string SplashRegionFail = "SplashRegion_Fail";

        public readonly static string SplashRegionUserInitialize = "SplashRegion_User_Initialize";

        public readonly static string SplashRegionNetworkInitialize = "SplashRegion_Network_Initialize";

        public readonly static string SplashRegionAIInitialize = "SplashRegion_AI_Initialize";
        public readonly static string SplashRegionAIDownloadStart = "SplashRegion_AI_DownloadStart";
        public readonly static string SplashRegionAILoad = "SplashRegion_AI_Load";
        public readonly static string SplashRegionAIDatabaseUpdate = "SplashRegion_AI_DatabaseUpdate";
        public readonly static string SplashRegionAIDatabaseLoad = "SplashRegion_AI_DatabaseLoad";



    }

}
