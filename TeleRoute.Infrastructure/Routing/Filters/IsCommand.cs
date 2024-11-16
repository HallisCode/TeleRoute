using System;
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

        public bool IsMatch(Update update)
        {
            if (update.Message.Text.StartsWith('/' + _command))
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