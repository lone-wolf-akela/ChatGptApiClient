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
        Receiving
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
        _ => throw new InvalidOperationException()
    };
    public Brush StatusColor => Status switch
    {
        StatusEnum.Idle => System.Windows.Application.Current.FindResource("PrimaryTextBrush") as Brush ?? Brushes.Black,
        StatusEnum.Sending => System.Windows.Application.Current.FindResource("InfoBrush") as Brush ?? Brushes.DeepSkyBlue,
        StatusEnum.Receiving => System.Windows.Application.Current.FindResource("SuccessBrush") as Brush ?? Brushes.LimeGreen,
        _ => throw new InvalidOperationException()
    };

    public NetStatus()
    {
        ThemeUpdater.ThemeChanged += () => OnPropertyChanged(nameof(StatusColor));
    }
}