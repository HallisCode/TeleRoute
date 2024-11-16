using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleRoute.Core.Routing.Filters
{
    public interface IFilter
    {
        UpdateType? AllowedType { get; }
        bool IsMatch(Update update);
        bool IsTypeAllowed(UpdateType type);
    }
}