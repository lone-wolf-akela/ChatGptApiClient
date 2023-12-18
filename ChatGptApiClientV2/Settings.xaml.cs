using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows;

namespace ChatGptApiClientV2;

/// <summary>
/// Settings.xaml 的交互逻辑
/// </summary>
[INotifyPropertyChanged]
public partial class Settings
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
        Config.AzureDeploymentList.Add(TxtbxAddAzureDeploymentId.Text);
        TxtbxAddAzureDeploymentId.Text = "";
    }

    private void btn_del_azure_deployment_id_Click(object sender, RoutedEventArgs e)
    {
        if (LstAzureDeploymentIds.SelectedItem is string id)
        {
            Config.AzureDeploymentList.Remove(id);
        }
    }
}