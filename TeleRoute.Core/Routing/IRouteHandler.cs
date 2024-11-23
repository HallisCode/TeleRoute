using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TeleRoute.Core.Routing
{
    /// <summary>
    /// Описывает работу маршрутизатора.
    /// </summary>
    public interface IRouteHandler
    {
        /// <summary>
        /// Обрабатывает запрос.
        /// </summary>
        /// <returns></returns>
        public Task Handle(Update update);
    }
}