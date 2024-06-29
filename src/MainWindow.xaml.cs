using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Synthesis;
using System.Linq;
using System.Diagnostics;
using Sharlayan.Core;

namespace PartyYomi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SpeechSynthesizer tts = new();
        private readonly Timer chatTimer;

        public GameContext gameContext;

        public MainWindow()
        {
            InitializeComponent();
            InitTTS();

            gameContext = GameContext.Instance();

            const int period = 500;
            chatTimer = new Timer(UpdateChat, null, 0, period);
        }

        private void UpdateChat(object? state)
        {
            if (ChatQueue.q.Any())
            {
                var chat = ChatQueue.q.Take();
                int.TryParse(chat.Code, System.Globalization.NumberStyles.HexNumber, null, out var intCode);
                var code = (ChatCode)intCode;
                if (code == ChatCode.Party) // || code == ChatCode.Say)
                {
                    string line = chat.Line;
                    ChatLogItem decodedChat = chat.Bytes.DecodeAutoTranslate();

                    var author = decodedChat.Line.RemoveAfter(":");
                    var sentence = decodedChat.Line.RemoveBefore(":");

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        chatBox.Text += $"{sentence}{Environment.NewLine}";
                    }); 
                    tts.SpeakAsync(sentence);
                }
            }
        }

        private void InitTTS()
        {
            tts.SetOutputToDefaultAudioDevice();
            var list = tts.GetInstalledVoices().ToList();
            // Exit if "Microsoft Haruka Desktop" is not installed
            if (list.Find(x => x.VoiceInfo.Name == "Microsoft Haruka Desktop") == null)
            {
                if (MessageBox.Show("Microsoft Haruka Desktop 음성을 설치해주세요.") == MessageBoxResult.OK)
                {
                    var ps = new ProcessStartInfo("https://support.microsoft.com/ko-kr/topic/%EC%9D%8C%EC%84%B1%EC%9D%84-%EC%9C%84%ED%95%9C-%EC%96%B8%EC%96%B4-%ED%8C%A9-%EB%8B%A4%EC%9A%B4%EB%A1%9C%EB%93%9C-24d06ef3-ca09-ddcc-70a0-63606fd16394")
                    {
                        UseShellExecute = true,
                        Verb = "open"
                    };
                    Process.Start(ps);
                }
                Application.Current.Shutdown();
            }

            tts.SelectVoice("Microsoft Haruka Desktop");

            // Speak in different thread
            tts.SpeakAsync("パーティー読みを開始します");
        }
    }
}