using System.Windows.Controls;
using System.Windows;

namespace MZ.Resource.Managers
{
    /// <summary>
    /// PasswordBox의 Password 바인딩을 지원하는 Property Manager
    /// </summary>
    public static class PasswordManager
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordManager), new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));
        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordManager), new PropertyMetadata(false, Attach));
        public static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordManager));

        public static void SetAttach(DependencyObject d, bool value)
        {
            d.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject d)
        {
            return (bool)d.GetValue(AttachProperty);
        }

        public static string GetPassword(DependencyObject d)
        {
            return (string)d.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject d, string value)
        {
            d.SetValue(PasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject d)
        {
            return (bool)d.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject d, bool value)
        {
            d.SetValue(IsUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordChanged;

                if (!(bool)GetIsUpdating(passwordBox))
                {
                    passwordBox.Password = (string)e.NewValue;
                }
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void Attach(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if (passwordBox == null)
                {
                    return;
                }

                if ((bool)e.OldValue)
                {
                    passwordBox.PasswordChanged -= PasswordChanged;
                }

                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordChanged;
                }
            }
        }

        private static void PasswordChanged(object d, RoutedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                SetIsUpdating(passwordBox, true);
                SetPassword(passwordBox, passwordBox.Password);
                SetIsUpdating(passwordBox, false);
            }
        }
    }
}
