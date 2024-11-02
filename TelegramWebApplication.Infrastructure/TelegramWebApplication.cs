using System;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleNetFramework.Core.Server;
using SimpleNetFramework.Infrastructure;
using SimpleNetFramework.Infrastructure.Server;
using Telegram.Bot.Types;


namespace TelegramWebApplication.Infrastructure
{
    public class TelegramWebApplication : WebApplicationBase<Update>, ITelegramWebApplication
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

        protected virtual IHttpResponse GetResponse200()
        {
            return new HttpResponse(
                statusCode: 200,
                message: "OK",
                body: null,
                protocol: "HTTP/1.1"
            );
        }
    }
}