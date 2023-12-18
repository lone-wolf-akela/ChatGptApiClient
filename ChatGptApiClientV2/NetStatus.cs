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
        _ => throw new System.ComponentModel.InvalidEnumArgumentException()
    };
    public Brush StatusColor => Status switch
    {
        StatusEnum.Idle => Brushes.Black,
        StatusEnum.Sending => Brushes.Blue,
        StatusEnum.Receiving => Brushes.Green,
        _ => throw new System.ComponentModel.InvalidEnumArgumentException()
    };
}