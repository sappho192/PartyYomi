using Sharlayan.Enums;
using Sharlayan.Models.ReadResults;
using Sharlayan.Models;
using Sharlayan;
using System.Diagnostics;
using PartyYomi.Helpers;
using Serilog;

namespace PartyYomi.FFXIV
{
    public class GameContext
    {
        /* FFXIV stuff */
        public bool Attached { get; }
        public static MemoryHandler? CurrentMemoryHandler { get; set; }
        private static Process[]? processes;
        private readonly Timer? chatTimer;

        // For chatlog you must locally store previous array offsets and indexes in order to pull the correct log from the last time you read it.
        private static int _previousArrayIndex = 0;
        private static int _previousOffset = 0;

        private static GameContext? _instance;
        public static GameContext Instance()
        {
            return _instance ??= new GameContext();
        }

        protected GameContext()
        {
            if (AttachGame())
            {
                const int period = 500;
                chatTimer = new Timer(RefreshChat, null, 0, period);
                chatTimer.ToString();
                //Log.Debug($"New RefreshChat timer with period {period}ms");
            }
            else
            {
                App.RequestShutdown();
            }
        }

        private void RefreshChat(object? state)
        {
            UpdateChat();
        }

        private bool UpdateChat()
        {
            ChatLogResult readResult = CurrentMemoryHandler.Reader.GetChatLog(_previousArrayIndex, _previousOffset);
            _previousArrayIndex = readResult.PreviousArrayIndex;
            _previousOffset = readResult.PreviousOffset;
            if (!readResult.ChatLogItems.IsEmpty)
            {
                foreach (var item in readResult.ChatLogItems)
                {
                    ChatCode code = (ChatCode)int.Parse(item.Code, System.Globalization.NumberStyles.HexNumber);
                    //ProcessChatMsg(readResult.ChatLogItems[i]);
                    if ((int)code < 0x9F) // Skips battle log
                    {
                        if (code == ChatCode.GilReceive || code == ChatCode.Gather || code == ChatCode.FieldAttack || code == ChatCode.EmoteCustom) continue;
                        ChatQueue.oq.Enqueue(item);
                    }
                }
                return true;
            }
            return false;
        }

        [TraceMethod]
        private bool AttachGame()
        {
            string processName = "ffxiv_dx11";
            //Log.Debug($"Finding process {processName}");

            // ko client filtering
            processes = Process.GetProcessesByName(processName).Where(x => { try { return System.IO.File.Exists(x.MainModule.FileName.Replace("game\\ffxiv_dx11.exe", "boot\\ffxivboot.exe")); } catch { return false; } }).ToArray();

            if (processes.Length > 0)
            {
                // supported: English, Chinese, Japanese, French, German, Korean
                GameRegion gameRegion = GameRegion.Global;
                GameLanguage gameLanguage = GameLanguage.English;
                // whether to always hit API on start to get the latest sigs based on patchVersion, or use the local json cache (if the file doesn't exist, API will be hit)
                bool useLocalCache = true;
                // patchVersion of game, or latest
                string patchVersion = "latest";
                Process process = processes[0];
                ProcessModel processModel = new()
                {
                    Process = process
                };

                var configuration = new SharlayanConfiguration
                {
                    ProcessModel = processModel,
                    GameLanguage = gameLanguage,
                    GameRegion = gameRegion,
                    PatchVersion = patchVersion,
                    UseLocalCache = useLocalCache
                };
                CurrentMemoryHandler = SharlayanMemoryManager.Instance.AddHandler(configuration);

                //Log.Debug($"Attached {processName}.exe ({gameLanguage})");
                //MessageBox.Show($"파티요미를 실행합니다.");

                return true;
            }
            else
            {
                string message = "파판을 먼저 켠 다음에 파티요미를 실행해주세요.";
                Log.Error(message);
                MessageBox.Show(message);
                return false;
            }
        }
    }
}
