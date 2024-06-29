using Sharlayan.Core;
using System.Collections.Concurrent;

namespace PartyYomi
{
    public class ChatQueue
    {
        public static BlockingCollection<ChatLogItem> q = new BlockingCollection<ChatLogItem>(new ConcurrentQueue<ChatLogItem>());
        public static string lastMsg = "";
    }
}
