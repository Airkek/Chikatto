using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Database.Models;
using Chikatto.Utils;

namespace Chikatto.Objects
{
    public class OnlineManager
    {
        private readonly Dictionary<string, int> OsuTokens = new(); // <Token, UserId>
        private readonly Dictionary<string, int> SafeNames = new(); // <SafeName, UserId>
        private readonly Dictionary<int, Presence> Users = new(); // <UserId, Presence>

        public int Count => OsuTokens.Count;

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
        }

        public async Task AddUser(int id, string token)
        {
            var presence = await Presence.FromDatabase(id);
            presence.Token = token;

            await AddUser(presence);
        }

        public async Task RemoveUserById(int id)
        {
            var user = GetById(id);
            if(user == null)
                return;

            OsuTokens.Remove(user.Token);
            SafeNames.Remove(user.User.SafeName);
            Users.Remove(id);
        }

        public async Task RemoveUserByToken(string token)
        {
            var user = GetByToken(token);
            if(user == null)
                return;

            OsuTokens.Remove(user.Token);
            SafeNames.Remove(user.User.SafeName);
            Users.Remove(user.User.Id);
        }

        public async Task ClearTrash()
        {
            foreach (var presence in Users.Cast<Presence>().Where(presence => presence.LastPong < new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() - 300))
            {
                OsuTokens.Remove(presence.Token);
                SafeNames.Remove(presence.User.SafeName);
                Users.Remove(presence.User.Id);
            }
        }

        public Presence GetById(int id) => Users.ContainsKey(id) ? Users[id] : null;
        public Presence GetByToken(string token) => OsuTokens.ContainsKey(token) ? GetById(OsuTokens[token]) : null;
        public Presence GetBySafeName(string safename) => SafeNames.ContainsKey(safename) ? GetById(SafeNames[safename]) : null;
        public Presence GetByName(string name) => GetBySafeName(Auth.GetSafeName(name));
    }
}