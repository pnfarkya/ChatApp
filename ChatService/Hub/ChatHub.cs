using ChatCommon.Models;
using ChatService.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Hub
{
    /// <summary>
    /// Chat Hub managing each client state on real-time
    /// </summary>
    public class ChatHub : Hub<IClientManager>
    {
        #region Private Properties

        private static ConcurrentDictionary<string, UserDetailsModel> ActiveUsers = new ConcurrentDictionary<string, UserDetailsModel>();

        #endregion

        #region Service Methods

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = ActiveUsers.FirstOrDefault(c => c.Value.ConnectionId == Context.ConnectionId).Key;
            if (user != null)
            {
                Clients.Others.UserDisconnect(user);
                Console.WriteLine($"User {user} disconnected");
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var user = ActiveUsers.FirstOrDefault(c => c.Value.ConnectionId == Context.ConnectionId).Key;
            if (user != null)
            {
                Clients.Others.UserReconnect(user);
                Console.WriteLine($"User {user} Reconnected");
            }
            return base.OnReconnected();
        }

        public List<UserDetailsModel> Login(string id, string displayName)
        {
            try
            {
                if (!ActiveUsers.ContainsKey(id))
                {
                    var user = new UserDetailsModel()
                    {
                        DisplayName = displayName,
                        Id = id,
                        ConnectionId = Context.ConnectionId
                    };
                    if (ActiveUsers.TryAdd(id, user))
                    {
                        Clients.CallerState.User = id;
                        Clients.Others.UserLogin(user);
                    }
                    Console.WriteLine($"New User {user} logged in");
                }
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.ToString());
            }
            return ActiveUsers.Values.ToList();
        }

        public void Logout(string id)
        {
            if (ActiveUsers.ContainsKey(id))
            {
                UserDetailsModel user = new UserDetailsModel();
                ActiveUsers.TryRemove(id, out user);
                Clients.Others.UserLogout(id);
                Console.WriteLine($"User {id} logged out");
            }
        }

        public void SendMessage(string receiverId, string message)
        {
            var sender = Clients.CallerState.User;
            if (ActiveUsers.ContainsKey(receiverId) && !string.IsNullOrWhiteSpace(message))
            {
                var receiver = new UserDetailsModel();
                ActiveUsers.TryGetValue(receiverId, out receiver);
                Clients.Client(receiver.ConnectionId).SendMessage(sender, message);
                Console.WriteLine($"Messaged received from {sender} and sent to {receiverId}");
            }
        }

        #endregion

        #region Overrides

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
