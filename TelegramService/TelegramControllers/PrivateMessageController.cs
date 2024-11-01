using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotClientExtended.Routing.Attributes;
using TelegramBotClientExtended.Routing.Filters;

namespace TelegramService.TelegramControllers
{
    [TelegramRoute]
    [PrivateChatFilter]
    public class PrivateMessageController
    {
        [TelegramRoute]
        public async Task Greeting(Update update)
        {
        }
    }
}