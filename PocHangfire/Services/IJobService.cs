namespace Aspnetcore.Hangfire.Services
{
    public interface IJobService
    {
        void FireAndForgetJob();
        void ReccuringJob();
        void DelayedJob();
        void ContinuationJob();
    }
}
