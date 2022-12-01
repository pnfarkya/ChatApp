using ChatClient.Services;
using Unity;
using Unity.Lifetime;

namespace ChatClient.Utils
{
    public class AppComposition
    {
        public static UnityContainer Container { get; set; }

        public AppComposition()
        {
            Container = new UnityContainer();
            Container.RegisterType<IChatService, ChatService>(new ContainerControlledLifetimeManager());
        }
    }
}
