using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Flurl;
using HandyControl.Themes;
using System.Windows.Media;

// ReSharper disable UnusedParameterInPartialMethod

namespace ChatGptApiClientV2;

public partial class Config : ObservableValidator
{
    [JsonIgnore]
    private static string ConfigPath => "config.json";

    [ObservableProperty]
    private double uiScale;
    partial void OnUiScaleChanged(double value)
    {
        UiScale = Math.Clamp(value, 0.5, 2.0);
        SaveConfig();
    }

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
    [NotifyDataErrorInfo]
    [StringLength(maximumLength: 64, MinimumLength = 0, ErrorMessage = "昵称长度必须在 0 到 64 之间")]
    [RegularExpression("^[a-zA-Z0-9_-]*$", ErrorMessage = "用户昵称只能包含英文字母、数字、下划线（_）和连接号（-）")]
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
        ValidateProperty(ServiceURL, nameof(ServiceURL));
        ValidateProperty(AzureEndpoint, nameof(AzureEndpoint));
        UpdateModelOptionList();
        UpdateModelVersionList();
        SaveConfig();
    }
        
    private string serviceURL;

    [Url(ErrorMessage = "必须为合法的 Http 或 Https 地址")]
    public string ServiceURL
    {
        get => ServiceProvider switch
        {
            ServiceProviderType.ArtonelicoOpenAIProxy => "https://www.artonelico.top/openai-proxy/v1",
            ServiceProviderType.OpenAI => "https://api.openai.com/v1",
            ServiceProviderType.Azure => AzureEndpoint,
            _ => serviceURL
        };
        set
        {
            if (SetProperty(ref serviceURL, value, true))
            {
                SaveConfig();
            }
        }
    }

    public enum AnthropicServiceProviderType
    {
        [Description("Artonelico Anthropic 代理")]
        ArtonelicAnthropicProxy,
        [Description("Anthropic 官方接口（需科学上网）")]
        Anthropic,
        [Description("其他")]
        Others
    }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AnthropicServiceURL))]
    [NotifyPropertyChangedFor(nameof(AnthropicServiceURLEditable))]
    [NotifyPropertyChangedFor(nameof(ModelOptions))]
    [NotifyPropertyChangedFor(nameof(SelectedModelIndex))]
    private AnthropicServiceProviderType anthropicServiceProvider;
    partial void OnAnthropicServiceProviderChanged(AnthropicServiceProviderType value)
    {
        ValidateProperty(AnthropicServiceURL, nameof(AnthropicServiceURL));
        UpdateModelOptionList();
        UpdateModelVersionList();
        SaveConfig();
    }

    private string anthropicServiceURL;

    [Url(ErrorMessage = "必须为合法的 Http 或 Https 地址")]
    public string AnthropicServiceURL
    {
        get => AnthropicServiceProvider switch
        {
            AnthropicServiceProviderType.ArtonelicAnthropicProxy => "https://www.artonelico.top/anthropic-proxy/v1",
            AnthropicServiceProviderType.Anthropic => "https://api.anthropic.com/v1",
            _ => anthropicServiceURL
        };
        set
        {
            if (SetProperty(ref anthropicServiceURL, value, true))
            {
                SaveConfig();
            }
        }
    }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Url(ErrorMessage = "必须为合法的 Http 或 Https 地址")]
    private string azureEndpoint;

    [JsonIgnore]
    public string DalleImageGenServiceURL => ServiceProvider switch
    {
        ServiceProviderType.Azure => Url.Combine(AzureEndpoint, $"openai/deployments/{AzureDalleDeploymentId}/images/generations?api-version=2024-02-01"),
        _ => Url.Combine(ServiceURL, "images/generations")
    };
    [JsonIgnore]
    public bool ServiceURLEditable => ServiceProvider switch
    {
        ServiceProviderType.ArtonelicoOpenAIProxy => false,
        ServiceProviderType.OpenAI => false,
        ServiceProviderType.Azure => true,
        ServiceProviderType.Others => true,
        _ => throw new InvalidOperationException()
    };

    [JsonIgnore]
    public bool AnthropicServiceURLEditable => AnthropicServiceProvider switch
    {
        AnthropicServiceProviderType.ArtonelicAnthropicProxy => false,
        AnthropicServiceProviderType.Anthropic => false,
        AnthropicServiceProviderType.Others => true,
        _ => throw new InvalidOperationException()
    };

    [ObservableProperty]
    // ReSharper disable once InconsistentNaming
    private string _API_KEY;
    partial void OnAPI_KEYChanged(string value) => SaveConfig();

    [ObservableProperty]
    private string azureAPIKey;
    partial void OnAzureAPIKeyChanged(string value) => SaveConfig();
    public ObservableCollection<string> AzureDeploymentList { get; } = [];

    [ObservableProperty]
    private string anthropicAPIKey;
    partial void OnAnthropicAPIKeyChanged(string value) => SaveConfig();

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
    private float temperature;
    partial void OnTemperatureChanged(float value) => SaveConfig();
    
    [ObservableProperty]
    private float topP;
    partial void OnTopPChanged(float value) => SaveConfig();

    [ObservableProperty]
    private float presencePenalty;
    partial void OnPresencePenaltyChanged(float value) => SaveConfig();

    [ObservableProperty]
    private int maxTokens;
    partial void OnMaxTokensChanged(int value) => SaveConfig();

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
                if (model.Provider == ModelInfo.ProviderEnum.OpenAI)
                {
                    ModelOptions.Add(model);
                }
            }
        }
        foreach (var model in ModelInfo.ModelList)
        {
            if (model.Provider == ModelInfo.ProviderEnum.Anthropic)
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

        if (SelectedModelType.Name == "azure")
        {
            foreach (var id in AzureDeploymentList)
            {
                var knowledgeCutoff =
                    id.Contains("gpt-4") && id.Contains("1106") ? new DateTime(2023, 4, 1) :
                    id.Contains("gpt-4") && id.Contains("0125") ? new DateTime(2023, 12, 1) :
                    id.Contains("gpt-4") && id.Contains("2024-04-09") ? new DateTime(2023, 12, 1) :
                    new DateTime(2021, 9, 1);
                var functionCallSupported = 
                    id.Contains("0301") ? false :
                    (id.Contains("vision") && id.Contains("1106")) ? false :
                    id.Contains("0314") ? false :
                    true;
                ModelVersionOptions.Add(new ModelVersionInfo
                {
                    ModelType = "azure",
                    Name = id,
                    Description = id,
                    KnowledgeCutoff = knowledgeCutoff,
                    FunctionCallSupported = functionCallSupported
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

    [ObservableProperty]
    private string pythonDllPath;
    partial void OnPythonDllPathChanged(string value) => SaveConfig();

    [ObservableProperty]
    private Utils.PythonEnv? pythonEnv;
    partial void OnPythonEnvChanged(Utils.PythonEnv? value) => SaveConfig();

    public Config()
    {
        theme = ThemeType.System;
        enableCustomThemeColor = false;
        customThemeColor = new SolidColorBrush(Color.FromRgb(0x2e, 0x6c, 0xf3));
        RefreshTheme();
        ThemeManager.Current.SystemThemeChanged += SystemThemeChanged;

        uiScale = 1.0;
        userNickName = "";
        serviceProvider = ServiceProviderType.ArtonelicoOpenAIProxy;
        serviceURL = "";
        anthropicServiceProvider = AnthropicServiceProviderType.ArtonelicAnthropicProxy;
        anthropicServiceURL = "";
        azureEndpoint = "";
        _API_KEY = "";
        anthropicAPIKey = "";
        azureAPIKey = "";
        azureDalleDeploymentId = "dall-e-3";
        googleSearchAPIKey = "";
        googleSearchEngineID = "";
        bingSearchAPIKey = "";
        wolframAlphaAppid = "";
        temperature = 0.5f;
        topP = 1.0f;
        presencePenalty = 0.0f;
        maxTokens = 0;
        seed = 0;
        enableMarkdown = false;
        selectedModelIndex = 0;
        selectedModelVersionIndex = 0;
        useRandomSeed = true;
        uploadHiresImage = false;
        pythonDllPath = "";
        pythonEnv = null;

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