using PartyYomi.Helpers;
using Serilog;
using System.Diagnostics;
using System.Speech.Synthesis;

namespace PartyYomi.Models
{
    public class GlobalSpeech
    {
        public static readonly SpeechSynthesizer defaultTTS = new();

        public static bool Initialize()
        {
            defaultTTS.SetOutputToDefaultAudioDevice();
            var list = defaultTTS.GetInstalledVoices().ToList();
            // Exit if "Microsoft Haruka Desktop" is not installed
            if (list.Find(x => x.VoiceInfo.Name == "Microsoft Haruka Desktop") == null)
            {
                string message = Localizer.getString("main.tts.not_installed");
                Log.Error(message);
                if (System.Windows.MessageBox.Show(message) == System.Windows.MessageBoxResult.OK)
                {
                    var ps = new ProcessStartInfo("https://github.com/sappho192/PartyYomi/wiki/%EC%9C%88%EB%8F%84%EC%9A%B0-TTS-%EC%84%A4%EC%B9%98%ED%95%98%EA%B8%B0")
                    {
                        UseShellExecute = true,
                        Verb = "open"
                    };
                    Process.Start(ps);
                }
                return false;
            }

            defaultTTS.SelectVoice("Microsoft Haruka Desktop");
            return true;
        }
    }
}
