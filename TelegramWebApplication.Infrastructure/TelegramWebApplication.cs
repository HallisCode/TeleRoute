using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleNetFramework.Core.Server;
using SimpleNetFramework.Infrastructure;
using SimpleNetFramework.Infrastructure.Server;
using Telegram.Bot.Types;
using TelegramWebApplication.Core;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;


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
                string _encodedBody = Encoding.UTF8.GetString(request.Request.Body);
                Update? update = JsonConvert.DeserializeObject<Update>(_encodedBody);

                if (update.Id == 0)
                {
                    return Task.CompletedTask;
                }

                _middlewares[0].Invoke(update);
            }

            return Task.CompletedTask;
        }

        protected virtual IHttpResponse GetResponse200()
        {
            HttpResponse response = new HttpResponse(
                statusCode: 200,
                message: "OK",
                body: null,
                protocol: "HTTP/1.1"
            );
            
            response.Headers.Add("Connection", "close");
            response.Headers.Add("Content-type", "application/json");

            return response;
        }
    }
}