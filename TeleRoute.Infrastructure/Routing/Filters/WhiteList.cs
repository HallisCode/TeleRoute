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

        public Task<bool> IsMatch(FilterContext filterContext)
        {
            if (_listUserId.Contains(filterContext.Update.Message.From.Id))
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