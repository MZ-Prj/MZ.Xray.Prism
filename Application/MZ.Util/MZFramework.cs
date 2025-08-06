using System.Windows.Media;
using System.Windows;

namespace MZ.Util
{
    /// <summary>
    /// WPF VisualTree 에서 FrameworkElement 탐색
    /// </summary>
    public class MZFramework
    {
        /// <summary>
        /// 지정된 이름을 가진 FrameworkElement를 VisualTree에서 재귀적으로 탐색하여 반환
        /// </summary>
        public static FrameworkElement FindControls(DependencyObject associatedObject, string name)
        {
            return FindChildByName(associatedObject, name);
        }

        /// <summary>
        /// 부모 DependencyObject로부터 이름으로 자식 FrameworkElement 찾기
        /// </summary>
        public static FrameworkElement FindChildByName(DependencyObject parent, string name)
        {
            if (parent == null)
            {
                return null;
            }
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement fe && fe.Name == name)
                {
                    return fe;
                }

                var result = FindChildByName(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

    }
}
