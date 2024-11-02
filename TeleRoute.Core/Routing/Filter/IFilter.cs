using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramWebApplication.Core.Routing.Filters
{
    public interface IFilter
    {
        UpdateType? AllowedType { get; }
        bool IsMatch(Update update);
        bool IsTypeConformsAllowedType(UpdateType type);
    }
}