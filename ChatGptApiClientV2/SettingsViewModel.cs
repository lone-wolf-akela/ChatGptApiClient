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
    private static bool IsDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

    // ReSharper disable once UnusedMember.Global
    public SettingsViewModel()
    {
        Debug.Assert(IsDesignMode);
        Config = new Config
        {
            ServiceProvider = Config.ServiceProviderType.ArtonelicoOpenAIProxy
        };
    }

    public SettingsViewModel(Config conf)
    {
        Config = conf;
    }

    [ObservableProperty] private Config config;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(BtnAddStopSequenceCommand))]
    private string textAddStopSequence = "";

    [RelayCommand]
    private void BtnAddStopSequence()
    {
        if (Config.StopSequences.Contains(TextAddStopSequence))
        {
            return;
        }

        const int maxStopSequences = 4;

        if (Config.StopSequences.Count >= maxStopSequences)
        {
            MessageBox.Show($"OpenAI API 仅支持最多{maxStopSequences}个停止序列", "错误",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Config.StopSequences.Add(TextAddStopSequence);
        TextAddStopSequence = "";
    }

    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<Utils.PythonEnv> PythonEnvs => Utils.FindPythonEnvs();

    public void UnLoadViewModel()
    {
    }
}