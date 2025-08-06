namespace MZ.Core
{

    /// <summary>
    /// 다른 모듈에 있는 View Name을 불러오기 위함
    /// </summary>
    public static class MZViewNames
    {
        public readonly static string DashboardControlView = "DashboardControlView";
        public readonly static string AnalysisControlView = "AnalysisControlView";
    }

    public static class MZRegionNames
    {
        //Common
        public readonly static string DashboardRegion = "DashboardRegion";
        public readonly static string SplashRegion = "SplashRegion";

        //Template
        public readonly static string DialogRegion = "DialogRegion";
        public readonly static string WindowDialogRegion = "WindowDialogRegion";
        public readonly static string LanguageRegion = "LanguageRegion";
        public readonly static string LoadingRegion = "LoadingRegion";

        public readonly static string UserLoginRegion = "UserLoginRegion";
        public readonly static string UserRegisterRegion = "UserRegisterRegion";

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

    }

    public static class MZResourceNames
    {
        public readonly static string SavePDF = "SavePDF";
        public readonly static string SaveScreen = "SaveScreen";
    }

}
