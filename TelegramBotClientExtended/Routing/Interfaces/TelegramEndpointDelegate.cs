using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBotClientExtended.Routing
{
    public delegate Task TelegramEndpointDelegate(Update update);
}