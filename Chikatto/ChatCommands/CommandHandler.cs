using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Chikatto.ChatCommands.Enums;
using Chikatto.Objects;

namespace Chikatto.ChatCommands
{
    public static class CommandHandler
    {
        private static readonly Dictionary<CommandAttribute, Handler> Commands = new();

        public static void Init()
        {
            var methods = Assembly.GetEntryAssembly().GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0);

            foreach (var method in methods)
            {
                var info = method.GetCustomAttributes(typeof(CommandAttribute), false)[0] as CommandAttribute;
                var handler = (Handler) Delegate.CreateDelegate(typeof(Handler), method);
                
                Commands[info] = handler;
            }
        }

        public static async Task Process(string message, Presence user, Channel channel = null)
        {
            if(string.IsNullOrWhiteSpace(message)) // message == command prefix lol
                return;
            
            var split = message.Trim().Split(' ');
            var trigger = split[0].ToLower();
            var args = split.Skip(1).ToArray();
            
            var commandRet = "Unknown command!";
            
            if (trigger == "help")
            {
                commandRet = "List of available commands:\r\n";
                commandRet += string.Join("\r\n", Commands
                    .Where(x => (x.Key.Privileges & user.User.Privileges) == x.Key.Privileges)
                    .Select(x => $"{Global.Config.CommandPrefix}{x.Key.Triggers[0]} - {x.Key.Description}"));
            }
            else
            {
                var (info, handler) = Commands.FirstOrDefault(x => x.Key.Triggers.Contains(trigger));
                
                if (info is not null)
                {
                    if ((user.User.Privileges & info.Privileges) == info.Privileges)
                    {
                        if (info.Type == CommandType.Multi && user.Match is null)
                            commandRet = "This command should be used in multiplayer room!";
                        else
                            commandRet = await handler(user, args);
                    }
                }
            }

            if (channel is null)
                await user.SendMessage(commandRet, Global.Bot);
            else
                await channel.WriteMessage(commandRet, Global.Bot);
        }
        
        public delegate Task<string> Handler(Presence user, string[] args);
    }
}