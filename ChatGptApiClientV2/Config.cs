using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace ChatGptApiClientV2
{
    public partial class ConfigType : ObservableObject
    {
        [JsonIgnore]
        public static string ConfigPath => "config.json";

        [ObservableProperty]
        private string userNickName;
        partial void OnUserNickNameChanged(string value) => SaveConfig();

        [ObservableProperty]
        private string _API_KEY;
        partial void OnAPI_KEYChanged(string value) => SaveConfig();

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

        [ObservableProperty]
        private double temperature;
        partial void OnTemperatureChanged(double value) => SaveConfig();

        [ObservableProperty]
        private int seed;
        partial void OnSeedChanged(int value) => SaveConfig();

        [ObservableProperty]
        private bool useRandomSeed;
        partial void OnUseRandomSeedChanged(bool value) => SaveConfig();

        [ObservableProperty]
        private bool enableMarkdown;
        partial void OnEnableMarkdownChanged(bool value) => SaveConfig();

        [JsonIgnore]
        public static ImmutableArray<ModelInfo> ModelOptions => ModelInfo.ModelList;
        [JsonIgnore]
        public ObservableCollection<ModelVersionInfo> ModelVersionOptions { get; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedModelType))]
        [NotifyPropertyChangedFor(nameof(SelectedModel))]
        [NotifyPropertyChangedFor(nameof(SelectedModelSupportTools))]
        private int selectedModelIndex;
        partial void OnSelectedModelIndexChanged(int value)
        {
            if (ModelOptions.Count() == 0)
            {
                SelectedModelIndex = -1;
            }
            else if (value < 0 || value >= ModelOptions.Count())
            {
                SelectedModelIndex = 0;
            }

            SaveConfig();
            UpdateModelVersionList();
        }
        private void UpdateModelVersionList()
        {
            ModelVersionOptions.Clear();

            if (SelectedModelType is not null)
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
            (SelectedModelIndex >= 0 && SelectedModelIndex < ModelOptions.Count()) ?
            ModelOptions[SelectedModelIndex] : null;
        [JsonIgnore]
        public ModelVersionInfo? SelectedModel =>
            (SelectedModelVersionIndex >= 0 && SelectedModelVersionIndex < ModelVersionOptions.Count) ?
            ModelVersionOptions[SelectedModelVersionIndex] : null;
        [JsonIgnore]
        public bool SelectedModelSupportTools => SelectedModel?.FunctionCallSupported ?? false;
        public ConfigType()
        {
            userNickName = string.Empty;
            _API_KEY = "";
            googleSearchAPIKey = "";
            googleSearchEngineID = "";
            bingSearchAPIKey = "";
            temperature = 1.0;
            seed = 0;
            enableMarkdown = false;
            selectedModelIndex = 0;
            selectedModelVersionIndex = 0;
            useRandomSeed = true;

            UpdateModelVersionList();
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
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
            };
            var result = JsonConvert.SerializeObject(this, settings);
            File.WriteAllText(ConfigPath, result);
        }
        public static ConfigType LoadConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                return new();
            }

            string saved_config = File.ReadAllText(ConfigPath);
            try
            {
                var contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    TypeNameHandling = TypeNameHandling.Auto,
                };
                var parsed_config = JsonConvert.DeserializeObject<ConfigType>(saved_config, settings);
                return parsed_config ?? new();
            }
            catch (JsonSerializationException exception)
            {
                MessageBox.Show($"Error: Invalid config file: {exception.Message}");
                return new();
            }
        }
    }
}
