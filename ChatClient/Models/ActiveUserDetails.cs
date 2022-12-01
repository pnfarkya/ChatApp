using ChatClient.Infra;
using System.Collections.ObjectModel;

namespace ChatClient.Models
{
    /// <summary>
    /// Contains Details for All Real-time Available user 
    /// </summary>
    public class ActiveUserDetails : ViewModel
    {
        #region Public Properties

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

        private ObservableCollection<MessageDetails> _messages;
        public ObservableCollection<MessageDetails> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        #endregion
    }
}
