using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatGptApiClientV2
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    [INotifyPropertyChanged]
    public partial class Settings : Window
    {
        [ObservableProperty]
        public ConfigType config;
        public Settings(ConfigType config)
        {
            InitializeComponent();
            Config = config;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
