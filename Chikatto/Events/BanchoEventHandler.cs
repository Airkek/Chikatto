using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events
{
    public static class BanchoEventHandler
    {
        private static readonly Dictionary<PacketType, KeyValuePair<bool, Handler>> Handlers = new();
        
#if DEBUG
        private static readonly List<PacketType> IgnoreLog = new()
        {
            PacketType.OsuPong,
            PacketType.OsuUserStatsRequest
        };
#endif

        public static void Init()
        {
            var methods = Assembly.GetEntryAssembly().GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(EventAttribute), false).Length > 0);

            foreach (var method in methods)
            {
                var info = method.GetCustomAttributes(typeof(EventAttribute), false)[0] as EventAttribute;
                var handler = (Handler) Delegate.CreateDelegate(typeof(Handler), method);
                
                Handlers[info.Type] = new KeyValuePair<bool, Handler>(info.Restricted, handler);
            }
        }

        public static async Task Handle(this Packet packet, Presence user)
        {
            if (!Handlers.ContainsKey(packet.Type))
            {
                XConsole.Log($"{user}: Not implemented packet: {packet}", back: ConsoleColor.Yellow);
                return;
            }
            
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            try
            {
                var (canUseRestricted, handler) = Handlers[packet.Type];
                
                if(user.Restricted && !canUseRestricted)
                    return;

                await using var reader = PacketReader.Create(packet);
                await handler.Invoke(reader, user);
                
#if DEBUG
                if(!IgnoreLog.Contains(packet.Type))
                    XConsole.Log($"{user}: Handled: {packet} (handle took {sw.Elapsed.TotalMilliseconds}ms)", back: ConsoleColor.Green);
#endif
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                XConsole.Log($"{user}: Handle failed: {packet}", back: ConsoleColor.Red);
            }
        }
        
        private delegate Task Handler(PacketReader packet, Presence presence);
    }
}