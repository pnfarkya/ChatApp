using System;
using System.ComponentModel;

namespace ChatClient.Infra
{
    public class ViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name = null)
        {
            if (name != null) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected virtual void OnDispose() { }

        void IDisposable.Dispose()
        {
            this.OnDispose();
        }
    }
}
