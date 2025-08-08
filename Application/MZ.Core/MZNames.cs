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
        public readonly static string SavePDF = "SavePDF";
        public readonly static string SaveScreen = "SaveScreen";

        //Common Button
        public readonly static string CommonRefresh = "CommonRefresh";
        public readonly static string CommonSearch = "CommonSearch";
        public readonly static string CommonRedo = "CommonRedo";
        public readonly static string CommonUndo = "CommonUndo";

        //Footer Button
        public readonly static string XrayRealtimeRegion = "XrayRealtimeRegion";
        public readonly static string XrayRealtimeRegion_AIOnOff = "XrayRealtimeRegion_AIOnOff";
        public readonly static string XrayRealtimeRegion_BrightDown = "XrayRealtimeRegion_BrightDown";
        public readonly static string XrayRealtimeRegion_BrighUp = "XrayRealtimeRegion_BrighUp";
        public readonly static string XrayRealtimeRegion_Color = "XrayRealtimeRegion_Color";
        public readonly static string XrayRealtimeRegion_ContrastDown = "XrayRealtimeRegion_ContrastDown";
        public readonly static string XrayRealtimeRegion_ContrastUp = "XrayRealtimeRegion_ContrastUp";
        public readonly static string XrayRealtimeRegion_FilterClear = "XrayRealtimeRegion_FilterClear";
        public readonly static string XrayRealtimeRegion_Gray = "XrayRealtimeRegion_Gray";
        public readonly static string XrayRealtimeRegion_Inorganic = "XrayRealtimeRegion_Inorganic";
        public readonly static string XrayRealtimeRegion_Metal = "XrayRealtimeRegion_Metal";
        public readonly static string XrayRealtimeRegion_Next = "XrayRealtimeRegion_Next";
        public readonly static string XrayRealtimeRegion_Organic = "XrayRealtimeRegion_Organic";
        public readonly static string XrayRealtimeRegion_Picker = "XrayRealtimeRegion_Picker";
        public readonly static string XrayRealtimeRegion_PlayStop = "XrayRealtimeRegion_PlayStop";
        public readonly static string XrayRealtimeRegion_Previous = "XrayRealtimeRegion_Previous";
        public readonly static string XrayRealtimeRegion_SaveImage = "XrayRealtimeRegion_SaveImage";
        public readonly static string XrayRealtimeRegion_Setting = "XrayRealtimeRegion_Setting";
        public readonly static string XrayRealtimeRegion_Zeffect = "XrayRealtimeRegion_Zeffect";
        public readonly static string XrayRealtimeRegion_ZoomIn = "XrayRealtimeRegion_ZoomIn";
        public readonly static string XrayRealtimeRegion_ZoomOut = "XrayRealtimeRegion_ZoomOut";

    }

}
