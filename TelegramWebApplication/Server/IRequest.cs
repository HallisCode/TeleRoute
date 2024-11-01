using System.Collections.Generic;
using System.Net.Http;

namespace TelegramWebApplication.Server
{
    public interface IRequest
    {
        HttpMethod Method { get; }
        string Route { get; }
        string Protocol { get; }

        Dictionary<string, string> Headers { get; }
        byte[] Body { get; }
    }
}