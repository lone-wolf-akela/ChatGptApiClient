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
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Flurl;
using HandyControl.Themes;
using System.Windows.Media;
using ColorCode.Common;

// ReSharper disable UnusedParameterInPartialMethod

namespace ChatGptApiClientV2;

public partial class Config : ObservableValidator
{
    [JsonIgnore]
    public static string UserAdvertisingId
    {
        get
        {
            var id = Windows.System.UserProfile.AdvertisingManager.AdvertisingId;
            return string.IsNullOrEmpty(id) ? "Anonymous" : id;
        }
    }
    [JsonIgnore] private static string ConfigPath => "config.json";

    public ObservableCollection<string> StopSequences { get; } = [];
    private void StopSequencesCollectionChanged(object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => SaveConfig();

    [ObservableProperty]
    public partial double UiScale { get; set; }

    partial void OnUiScaleChanged(double value)
    {
        UiScale = Math.Clamp(value, 0.5, 2.0);
        SaveConfig();
    }

    [ObservableProperty]
    public partial ThemeType Theme { get; set; }

    partial void OnThemeChanged(ThemeType value)
    {
        RefreshTheme();
        SaveConfig();
    }

    [ObservableProperty]
    public partial bool EnableCustomThemeColor { get; set; }

    partial void OnEnableCustomThemeColorChanged(bool value)
    {
        RefreshTheme();
        SaveConfig();
    }

    [ObservableProperty]
    public partial SolidColorBrush CustomThemeColor { get; set; }

    partial void OnCustomThemeColorChanged(SolidColorBrush value)
    {
        RefreshTheme();
        SaveConfig();
    }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [StringLength(maximumLength: 64, MinimumLength = 0, ErrorMessage = "昵称长度必须在 0 到 64 之间")]
    [RegularExpression("^[a-zA-Z0-9_-]*$", ErrorMessage = "用户昵称只能包含英文字母、数字、下划线（_）和连接号（-）")]
    public partial string UserNickName { get; set; }

    partial void OnUserNickNameChanged(string value) => SaveConfig();

    public enum ServiceProviderType
    {
        [Description("Artonelico OpenAI 代理")] ArtonelicoOpenAIProxy,
        [Description("OpenAI 官方接口（需科学上网）")] OpenAI,
        [Description("其他")] Others
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ServiceURL))]
    [NotifyPropertyChangedFor(nameof(ServiceURLEditable))]
    [NotifyPropertyChangedFor(nameof(ModelOptions))]
    [NotifyPropertyChangedFor(nameof(SelectedModelIndex))]
    public partial ServiceProviderType ServiceProvider { get; set; }

    partial void OnServiceProviderChanged(ServiceProviderType value)
    {
        ValidateProperty(ServiceURL, nameof(ServiceURL));
        UpdateModelVersionList();
        SaveConfig();
    }

    [Url(ErrorMessage = "必须为合法的 Http 或 Https 地址")]
    public string ServiceURL
    {
        get => ServiceProvider switch
        {
            ServiceProviderType.ArtonelicoOpenAIProxy => "https://www.artonelico.top/openai-proxy/v1",
            ServiceProviderType.OpenAI => "https://api.openai.com/v1",
            _ => field
        };
        set
        {
            if (SetProperty(ref field, value, true))
            {
                SaveConfig();
            }
        }
    }

    public enum GoogleGeminiServiceProviderType
    {
        [Description("Artonelico Gemini 代理")] ArtonelicoGeminiProxy,
        [Description("Google 官方接口（需科学上网）")] Google,
        [Description("其他")] Others
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(GoogleGeminiServiceURL))]
    [NotifyPropertyChangedFor(nameof(GoogleGeminiServiceURLEditable))]
    [NotifyPropertyChangedFor(nameof(ModelOptions))]
    [NotifyPropertyChangedFor(nameof(SelectedModelIndex))]
    public partial GoogleGeminiServiceProviderType GoogleGeminiServiceProvider { get; set; }

    partial void OnGoogleGeminiServiceProviderChanged(GoogleGeminiServiceProviderType value)
    {
        ValidateProperty(GoogleGeminiServiceURL, nameof(GoogleGeminiServiceURL));
        UpdateModelVersionList();
        SaveConfig();
    }

    [Url(ErrorMessage = "必须为合法的 Http 或 Https 地址")]
    public string GoogleGeminiServiceURL
    {
        get => GoogleGeminiServiceProvider switch
        {
            GoogleGeminiServiceProviderType.ArtonelicoGeminiProxy => "https://www.artonelico.top/gemini-proxy",
            GoogleGeminiServiceProviderType.Google => "https://generativelanguage.googleapis.com",
            _ => field
        };
        set
        {
            if (SetProperty(ref field, value, true))
            {
                SaveConfig();
            }
        }
    }

    [JsonIgnore]
    public bool GoogleGeminiServiceURLEditable => GoogleGeminiServiceProvider switch
    {
        GoogleGeminiServiceProviderType.ArtonelicoGeminiProxy => false,
        GoogleGeminiServiceProviderType.Google => false,
        GoogleGeminiServiceProviderType.Others => true,
        _ => throw new InvalidOperationException()
    };

    public enum AnthropicServiceProviderType
    {
        [Description("Artonelico Anthropic 代理")]
        ArtonelicAnthropicProxy,
        [Description("Anthropic 官方接口（需科学上网）")] Anthropic,
        [Description("其他")] Others
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AnthropicServiceURL))]
    [NotifyPropertyChangedFor(nameof(AnthropicServiceURLEditable))]
    [NotifyPropertyChangedFor(nameof(ModelOptions))]
    [NotifyPropertyChangedFor(nameof(SelectedModelIndex))]
    public partial AnthropicServiceProviderType AnthropicServiceProvider { get; set; }

    partial void OnAnthropicServiceProviderChanged(AnthropicServiceProviderType value)
    {
        ValidateProperty(AnthropicServiceURL, nameof(AnthropicServiceURL));
        UpdateModelVersionList();
        SaveConfig();
    }

    [Url(ErrorMessage = "必须为合法的 Http 或 Https 地址")]
    public string AnthropicServiceURL
    {
        get => AnthropicServiceProvider switch
        {
            AnthropicServiceProviderType.ArtonelicAnthropicProxy => "https://www.artonelico.top/anthropic-proxy/v1",
            AnthropicServiceProviderType.Anthropic => "https://api.anthropic.com/v1",
            _ => field
        };
        set
        {
            if (SetProperty(ref field, value, true))
            {
                SaveConfig();
            }
        }
    }

    public enum DeepSeekServiceProviderType
    {
        [Description("DeepSeek 官方接口")] DeepSeek,
        [Description("硅基流动")] SiliconFlow,
        [Description("NVIDIA NIM")] Nvidia,
        [Description("其他")] Others
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DeepSeekServiceURL))]
    [NotifyPropertyChangedFor(nameof(DeepSeekServiceURLEditable))]
    [NotifyPropertyChangedFor(nameof(ModelOptions))]
    [NotifyPropertyChangedFor(nameof(SelectedModelIndex))]
    public partial DeepSeekServiceProviderType DeepSeekServiceProvider { get; set; }

    partial void OnDeepSeekServiceProviderChanged(DeepSeekServiceProviderType value)
    {
        ValidateProperty(DeepSeekServiceURL, nameof(DeepSeekServiceURL));
        UpdateModelVersionList();
        SaveConfig();
    }

    [Url(ErrorMessage = "必须为合法的 Http 或 Https 地址")]
    public string DeepSeekServiceURL
    {
        get => DeepSeekServiceProvider switch
        {
            DeepSeekServiceProviderType.DeepSeek => "https://api.deepseek.com/v1",
            DeepSeekServiceProviderType.SiliconFlow => "https://api.siliconflow.cn/v1/",
            DeepSeekServiceProviderType.Nvidia => "https://integrate.api.nvidia.com/v1",
            _ => field
        };
        set
        {
            if (SetProperty(ref field, value, true))
            {
                SaveConfig();
            }
        }
    }

    [JsonIgnore]
    public string DalleImageGenServiceURL => Url.Combine(ServiceURL, "images/generations");

    [JsonIgnore]
    public bool ServiceURLEditable => ServiceProvider switch
    {
        ServiceProviderType.ArtonelicoOpenAIProxy => false,
        ServiceProviderType.OpenAI => false,
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

    [JsonIgnore]
    public bool DeepSeekServiceURLEditable => DeepSeekServiceProvider switch
    {
        DeepSeekServiceProviderType.DeepSeek => false,
        DeepSeekServiceProviderType.SiliconFlow => false,
        DeepSeekServiceProviderType.Nvidia => false,
        DeepSeekServiceProviderType.Others => true,
        _ => throw new InvalidOperationException()
    };

    [ObservableProperty]
    // ReSharper disable once InconsistentNaming
    public partial string API_KEY { get; set; }

    partial void OnAPI_KEYChanged(string value) => SaveConfig();

    [ObservableProperty]
    public partial string AnthropicAPIKey { get; set; }
    partial void OnAnthropicAPIKeyChanged(string value) => SaveConfig();

    [ObservableProperty]
    public partial string DeepSeekAPIKey { get; set; }
    partial void OnDeepSeekAPIKeyChanged(string value) => SaveConfig();

    [ObservableProperty]
    public partial string SiliconFlowAPIKey { get; set; }
    partial void OnSiliconFlowAPIKeyChanged(string value) => SaveConfig();

    [ObservableProperty]
    public partial bool SiliconFlowUseProModel { get; set; }
    partial void OnSiliconFlowUseProModelChanged(bool value) => SaveConfig();

    [ObservableProperty]
    public partial string NvidiaAPIKey { get; set; }
    partial void OnNvidiaAPIKeyChanged(string value) => SaveConfig();

    public string SelectedDeepSeekAPIKey => DeepSeekServiceProvider switch
    {
        DeepSeekServiceProviderType.DeepSeek => DeepSeekAPIKey,
        DeepSeekServiceProviderType.SiliconFlow => SiliconFlowAPIKey,
        DeepSeekServiceProviderType.Nvidia => NvidiaAPIKey,
        DeepSeekServiceProviderType.Others => DeepSeekAPIKey,
        _ => throw new InvalidOperationException()
    };

    [ObservableProperty]
    public partial string GoogleGeminiAPIKey { get; set; }
    partial void OnGoogleGeminiAPIKeyChanged(string value) => SaveConfig();

    /* Google Search Plugin Config */
    [ObservableProperty]
    public partial string GoogleSearchEngineID { get; set; }
    partial void OnGoogleSearchEngineIDChanged(string value) => SaveConfig();

    [ObservableProperty]
    public partial string GoogleSearchAPIKey { get; set; }
    partial void OnGoogleSearchAPIKeyChanged(string value) => SaveConfig();

    /*******************************/
    /* Bing Search Plugin Config */
    [ObservableProperty]
    public partial string BingSearchAPIKey { get; set; }
    partial void OnBingSearchAPIKeyChanged(string value) => SaveConfig();

    /*****************************/
    /* Wolfram Alpha Plugin Config*/
    [ObservableProperty]
    public partial string WolframAlphaAppid { get; set; }
    partial void OnWolframAlphaAppidChanged(string value) => SaveConfig();
    /******************************/


    [ObservableProperty]
    public partial float Temperature { get; set; }
    partial void OnTemperatureChanged(float value) => SaveConfig();

    [ObservableProperty]
    public partial float TopP { get; set; }
    partial void OnTopPChanged(float value) => SaveConfig();

    [ObservableProperty]
    public partial float PresencePenalty { get; set; }
    partial void OnPresencePenaltyChanged(float value) => SaveConfig();

    [ObservableProperty]
    public partial int MaxTokens { get; set; }
    partial void OnMaxTokensChanged(int value) => SaveConfig();

    public enum MarkdownRenderMode
    {
        Disabled,
        EnabledForAllMessages,
        EnabledForAssistantMessages
    }

    [ObservableProperty]
    public partial MarkdownRenderMode EnableMarkdown { get; set; }
    partial void OnEnableMarkdownChanged(MarkdownRenderMode value) => SaveConfig();

    [ObservableProperty]
    public partial bool UploadHiresImage { get; set; }
    partial void OnUploadHiresImageChanged(bool value) => SaveConfig();

    [ObservableProperty]
    public partial bool EnableThinking { get; set; }
    partial void OnEnableThinkingChanged(bool value) => SaveConfig();

    [ObservableProperty]
    public partial int ThinkingLength { get; set; }
    partial void OnThinkingLengthChanged(int value) => SaveConfig();

    [JsonIgnore] public ObservableCollection<ModelInfo> ModelOptions { get; } = [];

    private void UpdateModelOptionList()
    {
        ModelOptions.Clear();
        foreach (var model in ModelInfo.ModelList)
        {
            ModelOptions.Add(model);
        }

        ModelOptions.SortStable((a, b) => a.DisplayPriority.CompareTo(b.DisplayPriority));
        SelectedModelIndex = 0;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    [JsonIgnore] public ObservableCollection<ModelVersionInfo> ModelVersionOptions { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedModelType))]
    [NotifyPropertyChangedFor(nameof(SelectedModel))]
    [NotifyPropertyChangedFor(nameof(SelectedModelSupportTools))]
    public partial int SelectedModelIndex { get; set; }

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
        SelectedModelChangedEvent?.Invoke();
    }

    [JsonIgnore]
    private ThirdPartyModelInfo SelectedThirdPartyModel
    {
        get
        {
            if (SelectedModelType is null ||
                SelectedModelType.Name != "ThirdParty" ||
                SelectedModelVersionIndex < 0 ||
                SelectedModelVersionIndex >= ThirdPartyModels.Count)
            {
                throw new InvalidOperationException();
            }

            return ThirdPartyModels[SelectedModelVersionIndex];
        }
    }

    [JsonIgnore]
    public string OtherOpenAICompatModelProviderName => SelectedThirdPartyModel.Provider;

    [JsonIgnore]
    public string OtherOpenAICompatServiceURL => SelectedThirdPartyModel.ServiceURL;

    [JsonIgnore]
    public string OtherOpenAICompatModelAPIKey => SelectedThirdPartyModel.APIKey;

    [JsonIgnore]
    public string OtherOpenAICompatModelDisplayName => SelectedThirdPartyModel.DisplayName;

    private void UpdateModelVersionList()
    {
        ModelVersionOptions.Clear();
        if (SelectedModelType is null)
        {
            SelectedModelVersionIndex = -1;
            return;
        }

        if (SelectedModelType.Name == "ThirdParty")
        {
            foreach (var model in ThirdPartyModels)
            {
                var info = new ModelVersionInfo
                {
                    Description = model.DisplayName,
                    FunctionCallSupported = model.EnableToolUse,
                    KnowledgeCutoff = model.KnowledgeCutoff,
                    ModelType = "ThirdParty",
                    Name = model.Name,
                    Tokenizer = ModelVersionInfo.TokenizerEnum.Cl100KBase
                };
                ModelVersionOptions.Add(info);
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
    public partial int SelectedModelVersionIndex { get; set; }

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
        SelectedModelChangedEvent?.Invoke();
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

    public delegate void SelectedModelChangedHandler();
    public event SelectedModelChangedHandler? SelectedModelChangedEvent;

    [JsonIgnore] public bool SelectedModelSupportTools => SelectedModel?.FunctionCallSupported ?? false;

    [ObservableProperty]
    public partial string PythonDllPath { get; set; }
    partial void OnPythonDllPathChanged(string value) => SaveConfig();

    [ObservableProperty]
    public partial Utils.PythonEnv? PythonEnv { get; set; }
    partial void OnPythonEnvChanged(Utils.PythonEnv? value) => SaveConfig();

    public partial class ThirdPartyModelInfo : ObservableValidator
    {
        [JsonIgnore] public Config? Parent { get; set; }

        [ObservableProperty]
        [Url(ErrorMessage = "必须为合法的 Http 或 Https 地址")]
        public partial string ServiceURL { get; set; } = "http://127.0.0.1:8080";

        partial void OnServiceURLChanged(string value)
        {
            ValidateProperty(value, nameof(ServiceURL));
            Parent?.ThirdPartyModelsChanged();
        }

        [ObservableProperty]
        public partial string Name { get; set; } = "LLaMA";
        partial void OnNameChanged(string value) => Parent?.ThirdPartyModelsChanged();

        [ObservableProperty]
        public partial string DisplayName { get; set; } = "LLaMA";
        partial void OnDisplayNameChanged(string value) =>Parent?.ThirdPartyModelsChanged();

        [ObservableProperty]
        public partial string Provider { get; set; } = "Meta AI";
        partial void OnProviderChanged(string value) => Parent?.ThirdPartyModelsChanged();

        [ObservableProperty]
        public partial DateTime KnowledgeCutoff { get; set; } = new (2023, 1, 1);
        partial void OnKnowledgeCutoffChanged(DateTime value) => Parent?.ThirdPartyModelsChanged();

        [ObservableProperty]
        public partial bool EnableToolUse { get; set; } = false;
        partial void OnEnableToolUseChanged(bool value) => Parent?.ThirdPartyModelsChanged();

        [ObservableProperty]
        public partial string APIKey { get; set; } = "";
        partial void OnAPIKeyChanged(string value) => Parent?.ThirdPartyModelsChanged();
    }



    public ObservableCollection<ThirdPartyModelInfo> ThirdPartyModels { get; } = [];

    public void AddThirdPartyModel()
    {
        ThirdPartyModels.Add(new ThirdPartyModelInfo { Parent = this });
    }

    public void RemoveThirdPartyModels(IEnumerable<ThirdPartyModelInfo> models)
    {
        // clone the list to avoid modification while iterating
        var clonedModels = models.ToList();
        foreach (var model in clonedModels)
        {
            ThirdPartyModels.Remove(model);
        }
    }

    private void ThirdPartyModelsCollectionChanged(object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => ThirdPartyModelsChanged();

    private void ThirdPartyModelsChanged()
    {
        UpdateModelVersionList();
        SaveConfig();
    }

    public Config()
    {
        Theme = ThemeType.System;
        EnableCustomThemeColor = false;
        CustomThemeColor = new SolidColorBrush(Color.FromRgb(0x2e, 0x6c, 0xf3));
        RefreshTheme();
        ThemeManager.Current.SystemThemeChanged += SystemThemeChanged;

        UiScale = 1.0;
        UserNickName = "";
        ServiceProvider = ServiceProviderType.ArtonelicoOpenAIProxy;
        ServiceURL = "";
        AnthropicServiceProvider = AnthropicServiceProviderType.ArtonelicAnthropicProxy;
        AnthropicServiceURL = "";
        DeepSeekServiceProvider = DeepSeekServiceProviderType.DeepSeek;
        DeepSeekServiceURL = "";
        GoogleGeminiServiceProvider = GoogleGeminiServiceProviderType.ArtonelicoGeminiProxy;
        GoogleGeminiServiceURL = "";
        API_KEY = "";
        AnthropicAPIKey = "";
        DeepSeekAPIKey = "";
        SiliconFlowAPIKey = "";
        SiliconFlowUseProModel = false;
        NvidiaAPIKey = "";
        GoogleGeminiAPIKey = "";
        GoogleSearchAPIKey = "";
        GoogleSearchEngineID = "";
        BingSearchAPIKey = "";
        WolframAlphaAppid = "";
        Temperature = 0.5f;
        TopP = 1.0f;
        PresencePenalty = 0.0f;
        MaxTokens = 0;
        EnableMarkdown = MarkdownRenderMode.EnabledForAssistantMessages;
        SelectedModelIndex = 0;
        SelectedModelVersionIndex = 0;
        UploadHiresImage = false;
        PythonDllPath = "";
        PythonEnv = null;
        EnableThinking = false;
        ThinkingLength = 4096;

        StopSequences.CollectionChanged += StopSequencesCollectionChanged;
        ThirdPartyModels.CollectionChanged += ThirdPartyModelsCollectionChanged;

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
            if (parsedConfig is null)
            {
                return new Config();
            }
            foreach (var model in parsedConfig.ThirdPartyModels)
            {
                model.Parent = parsedConfig;
            }
            return parsedConfig;
        }
        catch (JsonSerializationException exception)
        {
            MessageBox.Show($"Error: Invalid config file: {exception.Message}");
            return new Config();
        }
    }
}