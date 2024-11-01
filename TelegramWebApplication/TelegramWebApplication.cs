using System.Threading.Tasks;
using SimpleNetFramework.WebApplication;
using TelegramWebApplication.Server;

namespace TelegramWebApplication
{
    public abstract class TelegramWebApplicationBase : WebApplicationBase, ITelegramWebApplication
    {
        protected ITelegramServer _server;

        public TelegramWebApplicationBase(ITelegramServer server)
        {
            _server = server;
        }

        public override async Task StartAsync()
        {
            _ChainMiddleware();
            
            _server.SetHandler(_Handler);

            await _server.StartAsync();
        }

        public override async Task StopAsync()
        {
            await _server.StopAsync();
        }

        protected Task _Handler(ITelegramServerRequest request)
        {
            request.Response = GetResponse200();

            if (_middlewares.Count > 0)
            {
                _middlewares[0].Invoke()
            }

            return Task.CompletedTask;
        }
        
        protected abstract IResponse GetResponse200();
    }
}