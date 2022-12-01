using ChatClient.Services;
using ChatClient.Utils;
using ChatClient.ViewModels;
using System.Windows;
using Unity;

namespace ChatClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UserLoginWindow : Window
    {
        public UserLoginWindow()
        {
            InitializeComponent();
            DataContext = new UserLoginViewModel(AppComposition.Container.Resolve<IChatService>());

            var vm = DataContext as UserLoginViewModel;

            vm.CloseRequest += (s, e) => this.Close();
        }
    }
}
