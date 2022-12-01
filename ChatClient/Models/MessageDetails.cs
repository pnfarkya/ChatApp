using ChatClient.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Models
{
    /// <summary>
    /// Message Details
    /// </summary>
    public class MessageDetails : ViewModel
    {
        #region Public Properties
        private bool _isIncoming;
        public bool IsIncoming
        {
            get { return _isIncoming; }
            set
            {
                _isIncoming = value;
                OnPropertyChanged(nameof(IsIncoming));
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

        #endregion
    }
}
