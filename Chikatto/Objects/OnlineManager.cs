using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Extensions;
using Chikatto.Utils;

namespace Chikatto.Objects
{
    public class OnlineManager
    {
        private readonly ConcurrentDictionary<string, int> OsuTokens = new(); // <Token, UserId>
        private readonly ConcurrentDictionary<string, int> SafeNames = new(); // <SafeName, UserId>
        private readonly ConcurrentDictionary<int, Presence> Users = new(); // <UserId, Presence>

        public int Online => OsuTokens.Count;

        public async Task AddPacketToAllUsers(Packet packet)
        {
            foreach (var (_, user) in Users)
            {
                user.WaitingPackets.Enqueue(packet);
            }
        }

        public async Task<List<Presence>> GetOnlineUsers() => Users.Select(x => x.Value).ToList();

        public async Task AddUser(Presence presence)
        {
            OsuTokens[presence.Token] = presence.Id;
            SafeNames[presence.User.SafeName] = presence.Id;
            Users[presence.Id] = presence;
            
            await AddPacketToAllUsers(await FastPackets.UserPresence(presence));
        }

        public async Task AddUser(int id, string token)
        {
            var presence = await Presence.FromDatabase(id);
            presence.Token = token;

            await AddUser(presence);
        }

        public async Task Remove(Presence user)
        {
            if(user is null)
                return;
            
            user.LastPong = 0;
            
            foreach (var (_, c) in user.JoinedChannels)
                await c.RemoveUser(user);

            OsuTokens.Remove(user.Token);
            SafeNames.Remove(user.User.SafeName);
            Users.Remove(user.Id);
        }

        public async Task RemoveUserById(int id)
        {
            var user = GetById(id);

            await Remove(user);
        }

        public async Task RemoveUserByToken(string token)
        {
            var user = GetByToken(token);
            await Remove(user);
        }

        public async Task ClearTrash()
        {
            foreach (var presence in Users.Select(x => x.Value).Where(presence => presence.LastPong < new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() - 300))
            {
                await Remove(presence);
                XConsole.Log($"{presence} has been removed from online users due to inactivity", ConsoleColor.Magenta);
            }
        }

        public Presence GetById(int id) => Users.ContainsKey(id) ? Users[id] : null;
        public Presence GetByToken(string token) => OsuTokens.ContainsKey(token) ? GetById(OsuTokens[token]) : null;
        public Presence GetBySafeName(string safename) => SafeNames.ContainsKey(safename) ? GetById(SafeNames[safename]) : null;
        public Presence GetByName(string name) => GetBySafeName(Auth.GetSafeName(name));
    }
}