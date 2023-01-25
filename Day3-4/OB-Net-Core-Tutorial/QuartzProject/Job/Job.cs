using Quartz;

namespace OB_Net_Core_Tutorial.QuartzProject.Jobs
{
    [DisallowConcurrentExecution]
    public class Job : IJob
    {

        private readonly ILogger _logger;

        public Job(ILogger<Job> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Current Date : {DateTime.UtcNow}");
            Console.Out.WriteLineAsync("masuk");
        }
    }
}
