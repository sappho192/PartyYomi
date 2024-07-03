using ObservableCollections;
using PartyYomi.Helpers;
using YamlDotNet.Serialization;

namespace PartyYomi.Models.Settings
{
    public class UILanguages : ISettingsChangedEvent
    {
        public UILanguage? CurrentLanguage
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
        private UILanguage? language;

        [YamlIgnore]
        public static ObservableList<UILanguage> LanguageList
        {
            get => languageList;
            set
            {
                if (value != languageList)
                {
                    languageList = value;
                }
            }
        }
        private static ObservableList<UILanguage> languageList =
        [
            new UILanguage { Code = "ko-KR", Name = "한국어(대한민국)" },
            new UILanguage { Code = "en-US", Name = "English" },
        ];

        public event SettingsChangedEventHandler OnSettingsChanged;
    }
}
