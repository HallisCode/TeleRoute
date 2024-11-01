using System;
using System.Threading.Tasks;

namespace TelegramWebApplication.Server
{
    public interface ITelegramServer
    {
        void SetHandler(Func<ITelegramServerRequest, Task> handler);
        Task StartAsync();
        Task StopAsync();
    }
}