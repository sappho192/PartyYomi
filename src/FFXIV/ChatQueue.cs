﻿using ObservableCollections;
using Sharlayan.Core;

namespace PartyYomi.FFXIV
{
    public class ChatQueue
    {
        public static readonly ObservableQueue<ChatLogItem> oq = new();

        private static readonly INotifyCollectionChangedSynchronizedView<ChatLogItem> chatLogItems = oq.CreateView(x => x).ToNotifyCollectionChanged();
        public static INotifyCollectionChangedSynchronizedView<ChatLogItem> ChatLogItems
        {
            get
            {
                return chatLogItems;
            }
        }
    }
}
