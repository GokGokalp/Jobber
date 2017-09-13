using System;
using System.Collections.Generic;
using NLog;

namespace Jobber.Core
{
    public abstract class JobProducerBase<TJob> : IJobProducer
    {
        private static readonly Logger Logger = LogManager.GetLogger(JobberConfiguration.JobName);

        protected abstract List<IJob<TJob>> GetJobs();

        public void ProduceJobs()
        {
            Logger.Info("[JobProducerBase<TJob>.ProduceJobs] method begin.");

            List<IJob<TJob>> jobs = GetJobs();

            Logger.Info($"Total job count: {jobs.Count}");

            int producedJobs = 0;
            jobs.ForEach(async job =>
            {
                try
                {
                    await JobberConfiguration.SendEndpoint.Send(job);
                    producedJobs++;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });

            if ((jobs.Count - producedJobs) > 0)
            {
                Logger.Error($"{jobs.Count - producedJobs} {JobberConfiguration.JobName} jobs could not be produced.");
            }

            Logger.Info($"Produced job count: {producedJobs}");
            Logger.Info("[JobProducerBase<TJob>.ProduceJobs] method end.");
        }
    }
}