using System.Windows.Media;
using System.Windows;

namespace MZ.Util
{
    public class MZFramework
    {
        public static FrameworkElement FindControls(DependencyObject associatedObject, string name)
        {
            return FindChildByName(associatedObject, name);
        }

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
