using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace ChatGptApiClientV2;

public partial class SettingsViewModel : ObservableObject
{
    private static bool IsDeignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());
    private void ConfigServiceProviderPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Config.ServiceProvider))
        {
            OnPropertyChanged(nameof(IsAzure));
        }
    }
    public SettingsViewModel()
    {
        Debug.Assert(IsDeignMode);
        Config = new Config
        {
            ServiceProvider = Config.ServiceProviderType.Azure
        };
    }
    public SettingsViewModel(Config conf)
    {
        Config = conf;
        Config.PropertyChanged += ConfigServiceProviderPropertyChanged;
    }

    [ObservableProperty]
    private Config config;
    public bool IsAzure => Config.ServiceProvider == Config.ServiceProviderType.Azure;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BtnAddAzureDeploymentIdCommand))]
    private string textAddAzureDeploymentId = "";
    
    private bool TextAddAzureDeploymentIdNotEmpty => !string.IsNullOrEmpty(TextAddAzureDeploymentId);
    [RelayCommand(CanExecute = nameof(TextAddAzureDeploymentIdNotEmpty))]
    private void BtnAddAzureDeploymentId()
    {
        if (Config.AzureDeploymentList.Contains(TextAddAzureDeploymentId))
        {
            return;
        }
        Config.AzureDeploymentList.Add(TextAddAzureDeploymentId);
        TextAddAzureDeploymentId = "";
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BtnDelAzureDeploymentIdCommand))]
    private int selectedAzureDeploymentIdIndex = -1;
    
    private bool SelectedAzureDeploymentIdIndexValid => SelectedAzureDeploymentIdIndex >= 0;
    [RelayCommand(CanExecute = nameof(SelectedAzureDeploymentIdIndexValid))]
    private void BtnDelAzureDeploymentId()
    {
        if (SelectedAzureDeploymentIdIndex >= 0)
        {
            Config.AzureDeploymentList.RemoveAt(SelectedAzureDeploymentIdIndex);
        }
    }

    public IEnumerable<Utils.PythonEnv> PythonEnvs => Utils.FindPythonEnvs();

    public void UnLoadViewModel()
    {
        Config.PropertyChanged -= ConfigServiceProviderPropertyChanged;
    }
}