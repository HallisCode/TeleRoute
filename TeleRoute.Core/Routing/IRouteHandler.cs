using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramWebApplication.Core.Routing
{
    public interface IRouteHandler
    {
        /// <summary>
        /// Обрабатывает запрос.
        /// </summary>
        /// <returns></returns>
        public Task Handle(Update update);
    }
}