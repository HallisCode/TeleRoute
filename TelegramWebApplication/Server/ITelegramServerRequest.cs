namespace TelegramWebApplication.Server
{
    public interface ITelegramServerRequest
    {
        IRequest Request { get; }
        IResponse Response { get; set; }

        bool isResponseSet { get; }
    }
}