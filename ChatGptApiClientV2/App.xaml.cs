using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ChatGptApiClientV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            AppDomain.CurrentDomain.UnhandledException += AppExceptionHandler;
            DispatcherUnhandledException += UIExceptionHandler;
        }
        private void UIExceptionHandler(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandyControl.Controls.MessageBox.Show(e.Exception.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
        private void AppExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            // dump to crash.log
            var ex = (Exception)e.ExceptionObject;
            System.IO.File.WriteAllText("crash.log", ex.ToString());
        }
    }
}
