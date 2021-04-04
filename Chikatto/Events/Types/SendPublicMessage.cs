using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
using Chikatto.ChatCommands;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class SendPublicMessage
    {
        [Event(PacketType.OsuSendPublicMessage, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            var message = reader.ReadBanchoObject<BanchoMessage>();

            if (!user.JoinedChannels.ContainsKey(message.To))
            {
                await user.Notify("Вы не находитесь в данном канале!");
                return;
            }

            var channel = user.JoinedChannels[message.To];

            await channel.WriteMessage(message.Body, user);
            
            if (message.Body.StartsWith(Global.Config.CommandPrefix))
                await CommandHandler.Process(message.Body.Substring(Global.Config.CommandPrefix.Length), user, channel);
            
            XConsole.Log($"{user} -> {message.To}: {message.Body}");
        }
    }
}