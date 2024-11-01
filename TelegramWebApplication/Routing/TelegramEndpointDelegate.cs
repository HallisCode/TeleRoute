using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramWebApplication.Routing
{
    public delegate Task TelegramEndpointDelegate(Update update);
}