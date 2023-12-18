using System;
using System.Windows;

namespace ChatGptApiClientV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += AppExceptionHandler;
            DispatcherUnhandledException += UiExceptionHandler;
        }
        private static void UiExceptionHandler(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandyControl.Controls.MessageBox.Show(e.Exception.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
        private static void AppExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            // dump to crash.log
            var ex = (Exception)e.ExceptionObject;
            System.IO.File.WriteAllText("crash.log", ex.ToString());
        }
    }
}
