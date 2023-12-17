using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private Config config;
        private void ConfigServiceProviderPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Config.ServiceProvider))
            {
                OnPropertyChanged(nameof(IsAzure));
            }
        }
        public bool IsAzure => Config.ServiceProvider == Config.ServiceProviderType.Azure;

        public Settings(Config conf)
        {
            config = conf;
            Config.PropertyChanged += ConfigServiceProviderPropertyChanged;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Config.PropertyChanged -= ConfigServiceProviderPropertyChanged;
        }

        private void btn_add_azure_deployment_id_Click(object sender, RoutedEventArgs e)
        {
            Config.AzureDeploymentList.Add(txtbx_add_azure_deployment_id.Text);
            txtbx_add_azure_deployment_id.Text = "";
        }

        private void btn_del_azure_deployment_id_Click(object sender, RoutedEventArgs e)
        {
            if (lst_azure_deployment_ids.SelectedItem is string id)
            {
                Config.AzureDeploymentList.Remove(id);
            }
        }
    }
}
