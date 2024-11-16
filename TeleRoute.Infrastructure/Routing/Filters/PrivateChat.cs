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

        public Task<bool> IsMatch(FilterContext filterContext)
        {
            if (filterContext.Update.Message.Chat.Type == ChatType.Private)
            {
                return Task.FromResult<bool>(true);
            }

            return Task.FromResult<bool>(false);
        }

        public bool IsTypeAllowed(UpdateType type)
        {
            if (AllowedType is null) return true;

            return AllowedType.Equals(type);
        }
    }
}