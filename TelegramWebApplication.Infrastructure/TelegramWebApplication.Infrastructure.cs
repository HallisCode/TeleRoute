using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using SimpleNetFramework.Core.Server;
using SimpleNetFramework.Infrastructure;
using Telegram.Bot.Types;


namespace TelegramWebApplication.Infrastructure
{
    public abstract class TelegramWebApplication : WebApplicationBase<Update>, ITelegramWebApplication
    {
        public TelegramWebApplication(IServer server, IServiceProvider provider) : base(server, provider)
        {
        }

        protected override Task HandleServerRequest(IServerRequest request)
        {
            request.Response = GetResponse200();

            if (_middlewares.Count > 0)
            {
                Update? update = (Update?)JsonSerializer.Deserialize(request.Request.Body, typeof(Update));

                if (update is null)
                {
                    return Task.CompletedTask;
                }

                _middlewares[0].Invoke(update);
            }

            return Task.CompletedTask;
        }

        protected abstract IHttpResponse GetResponse200();
    }
}