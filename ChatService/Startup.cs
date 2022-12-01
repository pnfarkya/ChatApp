using Owin;

namespace ChatService
{
    /// <summary>
    /// Startup class to start the service and host the service using owin Self-Hosting
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.MapSignalR("/user-messaging", new Microsoft.AspNet.SignalR.HubConfiguration());
        }
    }
}
