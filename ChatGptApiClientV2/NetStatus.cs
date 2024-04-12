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

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace ChatGptApiClientV2;

public partial class NetStatus : ObservableObject
{
    public enum StatusEnum
    {
        Idle,
        Sending,
        Receiving,
        Processing
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    [NotifyPropertyChangedFor(nameof(StatusColor))]
    private StatusEnum status = StatusEnum.Idle;

    [ObservableProperty]
    private string systemFingerprint = "";
    public string StatusText => Status switch
    {
        StatusEnum.Idle => "空闲，等待输入。",
        StatusEnum.Sending => "正在发送数据……",
        StatusEnum.Receiving => "正在接收数据……",
        StatusEnum.Processing => "正在处理数据……",
        _ => throw new InvalidOperationException()
    };
    public Brush StatusColor => Status switch
    {
        StatusEnum.Idle => System.Windows.Application.Current.FindResource("PrimaryTextBrush") as Brush ?? Brushes.Black,
        StatusEnum.Sending => System.Windows.Application.Current.FindResource("InfoBrush") as Brush ?? Brushes.DeepSkyBlue,
        StatusEnum.Receiving => System.Windows.Application.Current.FindResource("SuccessBrush") as Brush ?? Brushes.LimeGreen,
        StatusEnum.Processing => System.Windows.Application.Current.FindResource("WarningBrush") as Brush ?? Brushes.Orange,
        _ => throw new InvalidOperationException()
    };

    public NetStatus()
    {
        ThemeUpdater.ThemeChanged += () => OnPropertyChanged(nameof(StatusColor));
    }
}