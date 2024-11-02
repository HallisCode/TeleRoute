using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramWebApplication.Infrastructure.Routing
{
    public delegate Task EndpointDelegate(Update update);
}