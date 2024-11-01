using System.Collections.Generic;

namespace TelegramWebApplication.Server
{
    public interface IResponse
    {
        string Protocol { get; }
        int StatusCode { get; }
        string Message { get; }
        
        Dictionary<string, string> Headers { get; }
        byte[] Body { get; }
    }
}