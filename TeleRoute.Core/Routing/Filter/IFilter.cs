using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleRoute.Core.Routing.Filters
{
    public interface IFilter
    {
        UpdateType? AllowedType { get; }
        Task<bool> IsMatch(FilterContext filterContext);
        bool IsTypeAllowed(UpdateType type);
    }
}