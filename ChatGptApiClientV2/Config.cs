using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Windows;
using System.ComponentModel;
using Flurl;
using Newtonsoft.Json.Linq;
using HandyControl.Themes;
using System.Reflection;
using System.Windows.Media;

namespace ChatGptApiClientV2;

public partial class Config : ObservableObject
{
    [JsonIgnore]
    private static string ConfigPath => "config.json";

    [ObservableProperty]
    private ThemeType theme;
    partial void OnThemeChanged(ThemeType value)
    {
        RefreshTheme();
        SaveConfig();
    }

    [ObservableProperty]
    private bool enableCustomThemeColor;
    partial void OnEnableCustomThemeColorChanged(bool value)
    {
        RefreshTheme();
        SaveConfig();
    }

    [ObservableProperty]
    private SolidColorBrush customThemeColor;
    partial void OnCustomThemeColorChanged(SolidColorBrush value)
    {
        RefreshTheme();
        SaveConfig();
    }

    [ObservableProperty]
    private string userNickName;
    partial void OnUserNickNameChanged(string value) => SaveConfig();

    public enum ServiceProviderType
    {
        [Description("Artonelico OpenAI 代理")]
        ArtonelicoOpenAIProxy,
        [Description("OpenAI 官方接口（需科学上网）")]
        OpenAI,
        [Description("Microsoft Azure")]
        Azure,
        [Description("其他")]
        Others
    }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ServiceURL))]
    [NotifyPropertyChangedFor(nameof(ServiceURLEditable))]
    [NotifyPropertyChangedFor(nameof(ModelOptions))]
    [NotifyPropertyChangedFor(nameof(SelectedModelIndex))]
    private ServiceProviderType serviceProvider;
    partial void OnServiceProviderChanged(ServiceProviderType value)
    {
        UpdateModelOptionList();
        UpdateModelVersionList();
        SaveConfig();
    }
        
    private string serviceURL;
    public string ServiceURL
    {
        get => ServiceProvider switch
        {
            ServiceProviderType.ArtonelicoOpenAIProxy => "https://www.artonelico.top/openai-proxy",
            ServiceProviderType.OpenAI => "https://api.openai.com",
            _ => serviceURL
        };
        set
        {
            if (SetProperty(ref serviceURL, value))
            {
                SaveConfig();
            }
        }
    }
    [ObservableProperty]
    private string azureEndpoint;

    [JsonIgnore]
    public string OpenAIChatServiceURL => ServiceProvider switch
    {
        ServiceProviderType.Azure => Url.Combine(AzureEndpoint, $"openai/deployments/{SelectedModel?.Name}/chat/completions?api-version=2023-12-01-preview"),
        _ => Url.Combine(ServiceURL, "v1/chat/completions")
    };
    [JsonIgnore]
    public string DalleImageGenServiceURL => ServiceProvider switch
    {
        ServiceProviderType.Azure => Url.Combine(AzureEndpoint, $"openai/deployments/{AzureDalleDeploymentId}/images/generations?api-version=2023-12-01-preview"),
        _ => Url.Combine(ServiceURL, "v1/images/generations")
    };
    [JsonIgnore]
    public bool ServiceURLEditable => ServiceProvider switch
    {
        ServiceProviderType.ArtonelicoOpenAIProxy => false,
        ServiceProviderType.OpenAI => false,
        ServiceProviderType.Azure => true,
        ServiceProviderType.Others => true,
        _ => throw new NotImplementedException()
    };

    [ObservableProperty]
    private string _API_KEY;
    partial void OnAPI_KEYChanged(string value) => SaveConfig();

    [ObservableProperty]
    private string azureAPIKey;
    partial void OnAzureAPIKeyChanged(string value) => SaveConfig();
    public ObservableCollection<string> AzureDeploymentList { get; } = [];

    private void AzureDeploymentListCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (ServiceProvider == ServiceProviderType.Azure)
        {
            UpdateModelVersionList();
        }
    }

    [ObservableProperty]
    private string azureDalleDeploymentId;
    partial void OnAzureDalleDeploymentIdChanged(string value) => SaveConfig();

    /* Google Search Plugin Config */
    [ObservableProperty]
    private string googleSearchEngineID;
    partial void OnGoogleSearchEngineIDChanged(string value) => SaveConfig();
    [ObservableProperty]
    private string googleSearchAPIKey;
    partial void OnGoogleSearchAPIKeyChanged(string value) => SaveConfig();
    /*******************************/
    /* Bing Search Plugin Config */
    [ObservableProperty]
    private string bingSearchAPIKey;
    partial void OnBingSearchAPIKeyChanged(string value) => SaveConfig();
    /*****************************/
    /* Wolfram Alpha Plugin Config*/
    [ObservableProperty]
    private string wolframAlphaAppid;
    partial void OnWolframAlphaAppidChanged(string value) => SaveConfig();
    /******************************/


    [ObservableProperty]
    private double temperature;
    partial void OnTemperatureChanged(double value) => SaveConfig();
    
    [ObservableProperty]
    private double topP;
    partial void OnTopPChanged(double value) => SaveConfig();

    [ObservableProperty]
    private double presencePenalty;
    partial void OnPresencePenaltyChanged(double value) => SaveConfig();

    [ObservableProperty]
    private long maxTokens;
    partial void OnMaxTokensChanged(long value) => SaveConfig();

    [ObservableProperty]
    private int seed;
    partial void OnSeedChanged(int value) => SaveConfig();

    [ObservableProperty]
    private bool useRandomSeed;
    partial void OnUseRandomSeedChanged(bool value) => SaveConfig();

    [ObservableProperty]
    private bool enableMarkdown;
    partial void OnEnableMarkdownChanged(bool value) => SaveConfig();

    [ObservableProperty]
    private bool uploadHiresImage;
    partial void OnUploadHiresImageChanged(bool value) => SaveConfig();

    [JsonIgnore]
    public ObservableCollection<ModelInfo> ModelOptions { get; } = [];
    private void UpdateModelOptionList()
    {
        ModelOptions.Clear();
        if (ServiceProvider == ServiceProviderType.Azure)
        {
            ModelOptions.Add(new ModelInfo { Name = "azure", Description = "Azure OpenAI Service" });
        }
        else
        {
            foreach(var model in ModelInfo.ModelList)
            {
                ModelOptions.Add(model);
            }
        }
        SelectedModelIndex = 0;
    }

    [JsonIgnore]
    public ObservableCollection<ModelVersionInfo> ModelVersionOptions { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedModelType))]
    [NotifyPropertyChangedFor(nameof(SelectedModel))]
    [NotifyPropertyChangedFor(nameof(SelectedModelSupportTools))]
    private int selectedModelIndex;
    partial void OnSelectedModelIndexChanged(int value)
    {
        if (ModelOptions.Count == 0)
        {
            SelectedModelIndex = -1;
        }
        else if (value < 0 || value >= ModelOptions.Count)
        {
            SelectedModelIndex = 0;
        }

        SaveConfig();
        UpdateModelVersionList();
    }
    private void UpdateModelVersionList()
    {
        ModelVersionOptions.Clear();
        if (SelectedModelType is null)
        {
            SelectedModelVersionIndex = -1;
            return;
        }

        if (ServiceProvider == ServiceProviderType.Azure)
        {
            foreach(var id in AzureDeploymentList)
            {
                DateTime knowledgeCutoff = 
                    id.Contains("gpt-4") && id.Contains("1106") 
                        ? new DateTime(2023, 4, 1) 
                        : new DateTime(2021, 9, 1);
                ModelVersionOptions.Add(new ModelVersionInfo
                {
                    ModelType = "azure",
                    Name = id,
                    Description = id,
                    KnowledgeCutoff = knowledgeCutoff,
                    FunctionCallSupported = true
                });
            }
        }
        else 
        {
            var models = from model in ModelVersionInfo.VersionList
                where model.ModelType == SelectedModelType.Name
                select model;
            foreach (var model in models)
            {
                ModelVersionOptions.Add(model);
            }
        }
        SelectedModelVersionIndex = 0;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedModel))]
    [NotifyPropertyChangedFor(nameof(SelectedModelSupportTools))]
    private int selectedModelVersionIndex;
    partial void OnSelectedModelVersionIndexChanged(int value)
    {
        if (ModelVersionOptions.Count == 0)
        {
            SelectedModelVersionIndex = -1;
        }
        else if (value < 0 || value >= ModelVersionOptions.Count)
        {
            SelectedModelVersionIndex = 0;
        }
        SaveConfig();
    }
    [JsonIgnore]
    public ModelInfo? SelectedModelType =>
        SelectedModelIndex >= 0 && SelectedModelIndex < ModelOptions.Count 
            ? ModelOptions[SelectedModelIndex] 
            : null;
    [JsonIgnore]
    public ModelVersionInfo? SelectedModel =>
        SelectedModelVersionIndex >= 0 && SelectedModelVersionIndex < ModelVersionOptions.Count 
            ? ModelVersionOptions[SelectedModelVersionIndex] 
            : null;
    [JsonIgnore]
    public bool SelectedModelSupportTools => SelectedModel?.FunctionCallSupported ?? false;
    public Config()
    {
        theme = ThemeType.System;
        enableCustomThemeColor = false;
        customThemeColor = new SolidColorBrush(Color.FromRgb(0x2e, 0x6c, 0xf3));
        RefreshTheme();
        ThemeManager.Current.SystemThemeChanged += SystemThemeChanged;

        userNickName = string.Empty;
        serviceProvider = ServiceProviderType.ArtonelicoOpenAIProxy;
        serviceURL = "";
        azureEndpoint = "";
        _API_KEY = "";
        azureAPIKey = "";
        azureDalleDeploymentId = "dall-e-3";
        googleSearchAPIKey = "";
        googleSearchEngineID = "";
        bingSearchAPIKey = "";
        wolframAlphaAppid = "";
        temperature = 1.0;
        topP = 1.0;
        presencePenalty = 0.0;
        maxTokens = 0;
        seed = 0;
        enableMarkdown = false;
        selectedModelIndex = 0;
        selectedModelVersionIndex = 0;
        useRandomSeed = true;
        uploadHiresImage = false;

        AzureDeploymentList.CollectionChanged += AzureDeploymentListCollectionChanged;

        UpdateModelOptionList();
        UpdateModelVersionList();
    }
    public void RefreshTheme()
    {
        ThemeUpdater.UpdateTheme(Theme, EnableCustomThemeColor ? CustomThemeColor : null);
    }
    private void SystemThemeChanged(object? sender, HandyControl.Data.FunctionEventArgs<ThemeManager.SystemTheme> e)
    {
        RefreshTheme();
    }

    private void SaveConfig()
    {
        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
        var settings = new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented,
            StringEscapeHandling = StringEscapeHandling.Default,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        };
        var result = JsonConvert.SerializeObject(this, settings);
        File.WriteAllText(ConfigPath, result);
    }
    public static Config LoadConfig()
    {
        if (!File.Exists(ConfigPath))
        {
            return new Config();
        }

        var savedConfig = File.ReadAllText(ConfigPath) ?? throw new NullReferenceException();
        try
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                TypeNameHandling = TypeNameHandling.Auto
            };
            var parsedConfig = JsonConvert.DeserializeObject<Config>(savedConfig, settings);
            return parsedConfig ?? new Config();
        }
        catch (JsonSerializationException exception)
        {
            MessageBox.Show($"Error: Invalid config file: {exception.Message}");
            return new Config();
        }
    }
}