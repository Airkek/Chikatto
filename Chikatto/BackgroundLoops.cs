﻿using System.Threading;
using Chikatto.Objects;

namespace Chikatto
{
    public static class BackgroundLoops
    {
        public static void Cleaner()
        {
            while (true)
            {
                Thread.Sleep(60000);
                Global.OnlineManager.ClearTrash().GetAwaiter().GetResult();
            }
        }
    }
}