using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TeleRoute.Core.Routing.Filters;
namespace TeleRoute.Infrastructure.Routing.Filters
{
    public class GroupChatFilterAttribute : Attribute, IFilter
    {
        public UpdateType? AllowedType { get; }
        public Task<bool> IsMatchAsync(Update update)
        {
            if (update.Message.Chat.Type == ChatType.Group ||
                update.Message.Chat.Type == ChatType.Supergroup)
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