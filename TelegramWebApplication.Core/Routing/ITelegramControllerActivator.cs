using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramWebApplication.Infrastructure.Routing
{
    public interface ITelegramControllerActivator
    {
        object GetControllerForUpdateAsync(Update update);
    }
}