using ChatCommon.Models;

namespace ChatService.Models
{
    public interface IClientManager
    {
        void UserLogin(UserDetailsModel user);

        void UserLogout(string userId);

        void UserReconnect(string userId);

        void UserDisconnect(string userId);

        void SendMessage(string receiverId, string message);
    }
}
