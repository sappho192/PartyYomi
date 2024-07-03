using ObservableCollections;
using PartyYomi.Helpers;
using YamlDotNet.Serialization;

namespace PartyYomi.Models.Settings
{
    public class UILanguages : ISettingsChangedEvent
    {
        public string? CurrentLanguage
        {
            get => language;
            set
            {
                if (value != language)
                {
                    language = value;
                    OnSettingsChanged?.Invoke(this, nameof(CurrentLanguage), language);
                }
            }
        }
        private string? language;

        [YamlIgnore]
        public ObservableList<string> LanguageList
        {
            get => languageList;
            set
            {
                if (value != languageList)
                {
                    languageList = value;
                    OnSettingsChanged?.Invoke(this, nameof(LanguageList), languageList);
                }
            }
        }
        private ObservableList<string> languageList =
        [
            "English", "한국어"
        ];

        public event SettingsChangedEventHandler OnSettingsChanged;
    }
}
