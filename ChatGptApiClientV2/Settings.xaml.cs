using CommunityToolkit.Mvvm.ComponentModel;
using HandyControl.Controls;
using HandyControl.Tools;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace ChatGptApiClientV2;

/// <summary>
/// Settings.xaml 的交互逻辑
/// </summary>
[ObservableObject]
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

    private void BtnColorPicker_Click(object sender, RoutedEventArgs e)
    {
        var picker = SingleOpenHelper.CreateControl<ColorPicker>();
        picker.SelectedBrush = Config.CustomThemeColor;
        var window = new PopupWindow
        {
            PopupElement = picker, 
            Owner = this
        };

        const bool showBackground = false;
        
        window.Loaded += (_, _) =>
        {
            var targetElement = picker;
            if (showBackground == false)
            {
                // from HandyControl.Tools.ArithmeticHelper.CalSafePoint
                var point = targetElement.PointToScreen(new Point(0, 0));

                if (point.X < 0) { point.X = 0; }
                if (point.Y < 0) { point.Y = 0; }
                var dpi = VisualTreeHelper.GetDpi(window);

                // patch: add dpi scaling
                point.X /= dpi.DpiScaleX;
                point.Y /= dpi.DpiScaleY;

                var maxLeft = SystemParameters.WorkArea.Width -
                              ((double.IsNaN(window.PopupElement.Width) ? window.PopupElement.ActualWidth : window.PopupElement.Width) +
                               BorderThickness.Left + BorderThickness.Right);
                var maxTop = SystemParameters.WorkArea.Height -
                             ((double.IsNaN(window.PopupElement.Height) ? window.PopupElement.ActualHeight : window.PopupElement.Height) +
                              BorderThickness.Top + BorderThickness.Bottom);
                point = new Point(maxLeft > point.X ? point.X : maxLeft, maxTop > point.Y ? point.Y : maxTop);
                window.Left = point.X;
                window.Top = point.Y;
            }
        };

        picker.Confirmed += (_, _) =>
            {
                Config.CustomThemeColor = picker.SelectedBrush;
                window.Close();
            };
        picker.Canceled += (_, _) => window.Close();
        window.ShowDialog(BtnColorPicker, showBackground);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Config.RefreshTheme();
    }
}