using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TeleRoute.Core.Routing.Filters;

namespace TeleRoute.Infrastructure.Routing.Filters
{
    public class PrivateChatFilterAttribute : Attribute, IFilter
    {
        public UpdateType? AllowedType { get; } = UpdateType.Message;

        public bool IsMatch(Update update)
        {
            return update.Message.Chat.Type == ChatType.Private;
        }

        public bool IsTypeAllowed(UpdateType type)
        {
            if (AllowedType is null) return true;

            return AllowedType.Equals(type);
        }
    }
}