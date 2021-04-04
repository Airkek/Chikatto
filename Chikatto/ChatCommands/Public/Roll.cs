using System.Threading.Tasks;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands.Public
{
    public class Roll
    {
        [Command(new[] {"roll"}, "Roll :D")]
        public static async Task<string> Handle(Presence user, string[] args)
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
    }
}