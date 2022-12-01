using ChatClient.Infra;
using ChatClient.Models;
using ChatClient.Services;
using ChatClient.Utils;
using ChatCommon.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace ChatClient.ViewModels
{
    /// <summary>
    /// ChatViewModel class responsible for real time messaging also handles each user state in real-time
    /// </summary>
    public class ChatViewModel : ViewModel
    {
        #region Private Properties

        private readonly IChatService _svc;

        #endregion

        #region Constructor
        public ChatViewModel(string userId, string displayName, List<UserDetailsModel> activeUsers)
        {
            UserId = userId;
            DisplayName = displayName;
            ActiveUsers = new ObservableCollection<ActiveUserDetails>();
            DisplayedMessages = new ObservableCollection<MessageDetails>();
            _svc = AppComposition.Container.Resolve<IChatService>();

            _svc.NewMessage += OnNewMesssge;
            _svc.UserLoggedIn += OnUserCheckIn;
            _svc.UserLoggedOut += OnUserCheckOut;
            _svc.UserDisconnected += OnUserDisconnection;

            foreach (var user in activeUsers.Where(c => c.Id != userId))
            {
                ActiveUsers.Add(new ActiveUserDetails()
                {
                    UserId = user.Id,
                    DisplayName = user.DisplayName,
                    Messages = new ObservableCollection<MessageDetails>()
                });
            }
        }

        #endregion

        #region Bindbable Public Properties

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

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        private ObservableCollection<ActiveUserDetails> _activeUsers;
        public ObservableCollection<ActiveUserDetails> ActiveUsers
        {
            get { return _activeUsers; }
            set
            {
                _activeUsers = value;
                OnPropertyChanged(nameof(ActiveUsers));
            }
        }

        private ActiveUserDetails _selectedUser;
        public ActiveUserDetails SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (value?.UserId != _selectedUser?.UserId)
                {
                    _selectedUser = value;
                    OnPropertyChanged(nameof(SelectedUser));
                    Message = String.Empty;
                    DisplayedMessages.Clear();
                    if (_selectedUser != null)
                        DisplayedMessages.AddRange(_selectedUser.Messages);
                }
            }
        }

        private string _msg;
        public string Message
        {
            get { return _msg; }
            set
            {
                _msg = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private ObservableCollection<MessageDetails> _displayedMessages;
        public ObservableCollection<MessageDetails> DisplayedMessages
        {
            get { return _displayedMessages; }
            set
            {
                _displayedMessages = value;
                OnPropertyChanged(nameof(DisplayedMessages));
            }
        }

        #endregion

        #region Commands

        public ICommand SendMessageCmd => new RelayCommand<object>(OnSendMesssge, _ => SelectedUser != null && !string.IsNullOrEmpty(Message));
        public ICommand LogoutCmd => new RelayCommand<object>(OnLogout);

        #endregion

        #region Command Methods

        private async void OnSendMesssge(object obj)
        {
            try
            {
                await _svc.SendMessage(SelectedUser.UserId, Message);
                var messageDetails = new MessageDetails
                {
                    Message = Message,
                    IsIncoming = false,
                };
                DisplayedMessages.Add(messageDetails);
                SelectedUser.Messages.Add(messageDetails);
                Message = String.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eception on Sending Message {ex.Message}");
            }
        }

        private async void OnLogout(object obj)
        {
            try
            {
                await _svc.Logout(UserId);
                OnDispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eception on logout {ex.Message}");
            }
        }

        #endregion

        #region Service Callbacks

        private void OnNewMesssge(string senderId, string msg)
        {
            try
            {
                var sender = ActiveUsers.FirstOrDefault(c => c.UserId == senderId);
                if (sender != null)
                {
                    var messageDetails = new MessageDetails
                    {
                        IsIncoming = true,
                        Message = msg
                    };
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        sender.Messages.Add(messageDetails);
                        if (senderId == SelectedUser?.UserId)
                            DisplayedMessages.Add(messageDetails);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eception on OnNewMesssge {ex.Message}");
            }
        }
        private void OnUserCheckIn(UserDetailsModel user)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ActiveUsers.Add(new ActiveUserDetails()
                {
                    UserId = user.Id,
                    DisplayName = user.DisplayName,
                    Messages = new ObservableCollection<MessageDetails>()
                }); ;
            });
        }
        private void OnUserCheckOut(string userId)
        {
            var existingUser = ActiveUsers.FirstOrDefault(c => c.UserId == userId);
            if (existingUser != null)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ActiveUsers.Remove(existingUser);
                });
        }

        private void OnUserDisconnection(string userId)
        {
            var existingUser = ActiveUsers.FirstOrDefault(c => c.UserId == userId);
            if (existingUser != null)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ActiveUsers.Remove(existingUser);
                });
        }

        #endregion

        #region Overrides
        protected override void OnDispose()
        {
            _svc.NewMessage -= OnNewMesssge;
            _svc.UserLoggedIn -= OnUserCheckIn;
            _svc.UserLoggedOut -= OnUserCheckOut;
            _svc.UserDisconnected -= OnUserDisconnection;
            base.OnDispose();
        }

        #endregion
    }
}
