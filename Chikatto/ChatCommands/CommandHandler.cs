using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands
{
    public static class CommandHandler
    {
        private static readonly Dictionary<string[], Handler> Handlers = new() //<Triggers[], Handler> 
        {
            [new[] { "test" }] = async (_, _) => "Hello World!",
            [new[] { "roll" }] = Roll,
        };

        public static async Task<string> Roll(Presence user, string[] args)
        {
            var min = 0;
            var max = 100;
            
            switch (args.Length)
            {
                case 0:
                    break;
                
                case 1:
                    int.TryParse(args[0], out max);
                    break;
                
                default:
                    int.TryParse(args[0], out var first);
                    int.TryParse(args[1], out var second);

                    min = first > second ? second : first;
                    max = first > second ? first : second;
                    break;
            }

            var roll = RandomFabric.Next(min, max);

            return $"Your lucky number: {roll}";
        }

        public static async Task Process(string message, Presence user, Channel channel = null)
        {
            if(string.IsNullOrWhiteSpace(message)) // message == command prefix lol
                return;
            
            var split = message.Split(' ');
            var trigger = split[0].ToLower();
            var args = split.Skip(1).ToArray();

            var commandRet = "Неизвестная команда!";
            
            foreach (var (triggers, handler) in Handlers)
            {
                if (triggers.Contains(trigger))
                {
                    commandRet = await handler(user, args);
                    break;
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