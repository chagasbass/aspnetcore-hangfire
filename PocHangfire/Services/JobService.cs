using Hangfire;
using System;

namespace Aspnetcore.Hangfire.Services
{
    public class JobService : IJobService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public JobService(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        public void FireAndForgetJob() => _backgroundJobClient.Enqueue(() => Console.WriteLine("Hello from a Fire and Forget job!"));
        public void ReccuringJob() => _recurringJobManager.AddOrUpdate("jobId", () => Console.WriteLine("Hello from a Fire and Forget job!"), Cron.Minutely);
        public void DelayedJob() => _backgroundJobClient.Schedule(() => Console.WriteLine("Hello from a Delayed job! Testando aqui"), TimeSpan.FromSeconds(30));
        public void ContinuationJob()
        {
            var parentJobId = _backgroundJobClient.Enqueue(() => FireAndForgetJob());
            _backgroundJobClient.ContinueJobWith(parentJobId, () => Console.WriteLine("Hello from a Continuation job!"));
        }
    }
}
