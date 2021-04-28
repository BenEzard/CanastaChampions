using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvxCanastaChampions.Core.Services
{
    public class PlayAudioMessage : MvxMessage
    {
        public string FilePath
        {
            get;
            private set;
        }

        public PlayAudioMessage(object sender, string filePath) : base(sender)
        {
            FilePath = filePath;
        }

    }
}
