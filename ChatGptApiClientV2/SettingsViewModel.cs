/*
    ChatGPT Client V2: A GUI client for the OpenAI ChatGPT API (and also Anthropic Claude API) based on WPF.
    Copyright (C) 2024 Lone Wolf Akela

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

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