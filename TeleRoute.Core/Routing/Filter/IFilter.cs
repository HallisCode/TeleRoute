using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleRoute.Core.Routing.Filters
{
    /// <summary>
    /// Описывает фильтр для контроллера.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Какие типы обрабатывает данный фильтр.
        /// </summary>
        /// <remarks>Unknown -> значит все.</remarks>
        UpdateType? AllowedType { get; }
        
        /// <summary>
        /// Логика фильтра.
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        bool IsMatch(Update update);
        
        /// <summary>
        /// Проверяет, может ли данный фильтр обработать переданный тип.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsTypeAllowed(UpdateType type);
    }
}