using Microsoft.Owin.Hosting;
using System;
using System.Configuration;

namespace ChatService
{
    public class Program
    {
        static void Main(string[] args)
        {
            var hostUrl = ConfigurationManager.AppSettings["ChatHost"];

            using (WebApp.Start<Startup>(hostUrl))
            {
                Console.WriteLine($"Chat Service Started with {hostUrl}");
                Console.ReadLine();
            }
        }
    }
}
