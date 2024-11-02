using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TeleRoute.Infrastructure.Routing
{
    public delegate Task EndpointDelegate(Update update);
}