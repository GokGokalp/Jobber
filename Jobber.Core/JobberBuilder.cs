using System;

namespace Jobber.Core
{
    public class JobberBuilder
    {
        private static readonly Lazy<JobberBuilder> _Instance = new Lazy<JobberBuilder>(() => new JobberBuilder());

        private JobberBuilder()
        {
        }

        public static JobberBuilder Instance => _Instance.Value;

        #region Fluent Methods
        public JobberBuilder SetJobName(string jobName)
        {
            JobberConfiguration.JobName = jobName;

            return this;
        }

        public JobberBuilder EnableServiceRecovery(int restartDelayInMinutes)
        {
            JobberConfiguration.EnableServiceRecovery = true;
            JobberConfiguration.RestartDelayInMinutes = restartDelayInMinutes;

            return this;
        }

        public JobberProducerBuilder CreateJobProducer()
        {
            return JobberProducerBuilder.Instance;
        }

        public JobberConsumerBuilder CreateJobConsumer()
        {
            return JobberConsumerBuilder.Instance;
        }

        public JobberStandaloneWorkerBuilder CreateStandaloneJobWorker()
        {
            return JobberStandaloneWorkerBuilder.Instance;
        }
        #endregion
    }
}