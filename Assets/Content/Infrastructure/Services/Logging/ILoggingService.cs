namespace Content.Infrastructure.Services.Logging
{
    public interface ILoggingService : IService
    {
        void LogMessage(string message, object sender = null);
        void LogWarning(string message, object sender = null);
        void LogError(string message, object sender = null);
    }
}