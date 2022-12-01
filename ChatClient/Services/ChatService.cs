using ChatCommon.Models;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;

namespace ChatClient.Services
{
    public interface IChatService
    {
        event Action<UserDetailsModel> UserLoggedIn;
        event Action<string> UserLoggedOut;
        event Action<string> UserReconnected;
        event Action<string> UserDisconnected;

        event Action ConnectionClosed;
        event Action ConnectionRestored;
        event Action ConnectionReconnecting;

        event Action<string, string> NewMessage;


        Task Connect();
        Task<List<UserDetailsModel>> Login(string name, string displayName);
        Task Logout(string userId);
        Task SendMessage(string receiver, string msg);
    }

    /// <summary>
    /// Chat service class for SignalR connection
    /// </summary>
    public class ChatService : IChatService
    {
        #region Private Properties

        private readonly int _connectionLimit;
        private readonly string _url;
        private HubConnection _connection;
        private IHubProxy _proxy;

        #endregion

        #region Constructor 
        public ChatService()
        {
            _url = ConfigurationManager.AppSettings["ChatHost"];
            int.TryParse(ConfigurationManager.AppSettings["ConnectionLimit"], out _connectionLimit);

            _connection = new HubConnection(_url);
            _proxy = _connection.CreateHubProxy("ChatHub");

        }

        #endregion

        #region IChatService Interface Properties

        public event Action<UserDetailsModel> UserLoggedIn;
        public event Action<string> UserLoggedOut;
        public event Action<string> UserReconnected;
        public event Action<string> UserDisconnected;
        public event Action ConnectionClosed;
        public event Action ConnectionReconnecting;
        public event Action ConnectionRestored;
        public event Action<string, string> NewMessage;

        #endregion

        #region Service Methods

        public async Task Connect()
        {
            _proxy.On<UserDetailsModel>("UserLogin", (userDetails) => UserLoggedIn?.Invoke(userDetails));
            _proxy.On<string>("UserLogout", (userId) => UserLoggedOut?.Invoke(userId));
            _proxy.On<string, string>("SendMessage", (userId, message) => NewMessage?.Invoke(userId, message));
            _proxy.On<string>("UserReconnect", (userId) => UserReconnected?.Invoke(userId));
            _proxy.On<string>("UserDisconnect", (userId) => UserDisconnected?.Invoke(userId));
            _connection.Reconnecting += Reconnecting;
            _connection.Closed += Disconneted;
            _connection.Reconnected += Reconnected;

            ServicePointManager.DefaultConnectionLimit = _connectionLimit;
            await _connection.Start();
        }

        public async Task<List<UserDetailsModel>> Login(string name, string displayName)
        {
            return await _proxy.Invoke<List<UserDetailsModel>>("Login", new object[] { name, displayName });
        }

        public async Task Logout(string userId)
        {
            await _proxy.Invoke("Logout", userId);
        }

        public async Task SendMessage(string receiver, string msg)
        {
            await _proxy.Invoke("SendMessage", new object[] { receiver, msg });
        }

        public void Reconnected()
        {
            ConnectionRestored?.Invoke();
        }

        public void Disconneted()
        {
            ConnectionClosed.Invoke();
        }
        private void Reconnecting()
        {
            ConnectionReconnecting?.Invoke();
        }

        #endregion
    }
}
