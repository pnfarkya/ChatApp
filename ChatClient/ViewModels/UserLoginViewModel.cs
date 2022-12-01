using ChatClient.Infra;
using ChatClient.Services;
using ChatClient.Views;
using System;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    /// <summary>
    /// User login VM to set the user details on Chat Service
    /// </summary>
    public class UserLoginViewModel : ViewModel
    {
        #region Private Properties

        private IChatService _svc;
        private const string _onlineServiceState = "Online";

        #endregion

        #region Constructor
        public UserLoginViewModel(IChatService svc)
        {
            _svc = svc;
            ConnectionStatus = "Offline";
            _svc.ConnectionRestored += Reconnected;
            _svc.ConnectionClosed += Disconnected;
        }

        #endregion

        #region Bindbable Public Properties

        private string _connectionStatus;
        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                _connectionStatus = value;
                OnPropertyChanged(nameof(ConnectionStatus));
            }
        }

        private string _userId;
        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        public string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }



        public event EventHandler CloseRequest;

        #endregion

        #region Commands
        public ICommand LoginCommand => new RelayCommand<object>(OnLogin, CanExecuteLogin);

        public ICommand ConnectCommand => new RelayCommand<object>(OnConnect);

        #endregion

        #region Command Methods
        private async void OnLogin(object obj)
        {
            try
            {
                var activeUsers = await _svc.Login(UserId, DisplayName);
                ChatWindow chatWindow = new ChatWindow();
                chatWindow.Show();
                chatWindow.DataContext = new ChatViewModel(UserId, DisplayName, activeUsers);
                RaiseCloseRequest();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eception on login {ex.Message}");
            }
        }

        private bool CanExecuteLogin(object obj)
        {
            return !string.IsNullOrEmpty(_displayName) && !string.IsNullOrEmpty(_userId) && ConnectionStatus == _onlineServiceState;
        }

        private async void OnConnect(object obj)
        {
            try
            {
                await _svc.Connect();
                ConnectionStatus = _onlineServiceState;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eception on connecting service {ex.Message}");
            }
        }

        #endregion

        #region Callbacks
        private async void Reconnected()
        {
            ConnectionStatus = _onlineServiceState;
        }

        private async void Disconnected()
        {
            var connectionTask = _svc.Connect();
            await connectionTask.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    ConnectionStatus = _onlineServiceState;
                }
            });
        }
        private void RaiseCloseRequest()
        {
            var handler = CloseRequest;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion

        #region Overrides

        protected override void OnDispose()
        {
            _svc.ConnectionRestored -= Reconnected;
            _svc.ConnectionClosed -= Disconnected;
            base.OnDispose();
        }

        #endregion
    }
}
