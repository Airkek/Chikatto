using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
using Chikatto.ChatCommands;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class SendPrivateMessage
    {
        [Event(PacketType.OsuSendPrivateMessage, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            if (user.Silenced)
                return;

            var message = reader.ReadBanchoObject<BanchoMessage>();

            if (message.To == Global.Bot.Name)
            {
                if (message.Body.StartsWith(Global.Config.CommandPrefix))
                {
                    await CommandHandler.Process(message.Body.Substring(Global.Config.CommandPrefix.Length), user);
                }
                return;
            }
            
            var location = Global.OnlineManager.GetByName(message.To);

            if (location is null)
            {
                await user.Notify("Пользователь не в сети.");
                return;
            }

            await location.SendMessage(message.Body, user);

            if (message.Body.StartsWith(Global.Config.CommandPrefix))
                await CommandHandler.Process(message.Body.Substring(Global.Config.CommandPrefix.Length), user);
            
            XConsole.Log($"{user} -> {location}: {message.Body}");
        }
    }
}