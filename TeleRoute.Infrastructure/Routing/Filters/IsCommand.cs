using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TeleRoute.Core.Routing.Filters;

namespace TeleRoute.Infrastructure.Routing.Filters
{
    public class IsCommandFilterAttribute : Attribute, IFilter
    {
        private readonly string _command = "";

        public UpdateType? AllowedType { get; } = UpdateType.Message;

        
        public IsCommandFilterAttribute(string command = "")
        {
            _command = command;
        }

        public Task<bool> IsMatchAsync(Update update)
        {
            if (update.Message.Text.StartsWith('/' + _command))
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