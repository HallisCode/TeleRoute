using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TeleRoute.Core.Routing.Filters;

namespace TeleRoute.Infrastructure.Routing.Filters
{
    public class WhiteListFilterAttribute : Attribute, IFilter
    {
        private readonly long[] _listUserId = new long[0];

        public UpdateType? AllowedType { get; } = UpdateType.Message;


        public WhiteListFilterAttribute(params long[] listUserId)
        {
            _listUserId = listUserId;
        }

        public bool IsMatch(Update update)
        {
            if (_listUserId.Contains(update.Message.From.Id))
            {
                return true;
            }

            return false;
        }

        public bool IsTypeAllowed(UpdateType type)
        {
            if (AllowedType is null) return true;


            return AllowedType.Equals(type);
        }
    }
}