using System;
using Telegram.Bot.Types.Enums;

namespace TeleRoute.Infrastructure.Routing.Attributes
{
    public class AllowedUpdateTypeAttribute : Attribute
    {
        public UpdateType? AllowedType { get; }

        public AllowedUpdateTypeAttribute(UpdateType type)
        {
            AllowedType = type;
        }
    }
}