using System;
using System.Collections.Generic;
using NLog;
using System.Linq;
using Newtonsoft.Json;

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
                if (JobberConfiguration.IsProducerTransientHandleModOn)
                {
                    bool retryFlag = true;
                    int retryCount = 0;

                    while (retryFlag)
                    {
                        try
                        {
                            if(retryCount > JobberConfiguration.ProducerTransientHandleMaxRetryCount)
                            {
                                retryFlag = false;
                                return;
                            }

                            await JobberConfiguration.SendEndpoints.Values.First().Send(job);

                            retryFlag = false;
                            producedJobs++;
                        }
                        catch (Exception ex)
                        {
                            retryCount++;

                            ex.Data.Add(job.CorrelationId, JsonConvert.SerializeObject(job.Data));

                            Logger.Error(ex);
                        }
                    }
                }
                else
                {
                    try
                    {
                        await JobberConfiguration.SendEndpoints.Values.First().Send(job);

                        producedJobs++;
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add(job.CorrelationId, JsonConvert.SerializeObject(job.Data));

                        Logger.Error(ex);
                    }
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

    public abstract class JobMultipleProducerBase<TJob> : IJobProducer
    {
        private static readonly Logger Logger = LogManager.GetLogger(JobberConfiguration.JobName);

        protected abstract List<MultipleQueueJob<TJob>> GetJobs();

        public void ProduceJobs()
        {
            Logger.Info("[JobProducerBase<TJob>.ProduceJobs] method begin.");

            List<MultipleQueueJob<TJob>> jobs = GetJobs();

            Logger.Info($"Total job count: {jobs.Count}");

            int producedJobs = 0;
            jobs.ForEach(async job =>
            {
                if (JobberConfiguration.IsProducerTransientHandleModOn)
                {
                    bool retryFlag = true;
                    int retryCount = 0;

                    while (retryFlag)
                    {
                        try
                        {
                            if (retryCount > JobberConfiguration.ProducerTransientHandleMaxRetryCount)
                            {
                                retryFlag = false;
                                return;
                            }

                            await JobberConfiguration.SendEndpoints[job.QueueName].Send(job.Job);

                            retryFlag = false;
                            producedJobs++;
                        }
                        catch (Exception ex)
                        {
                            retryCount++;

                            ex.Data.Add(job.Job.CorrelationId, JsonConvert.SerializeObject(job.Job.Data));

                            Logger.Error(ex);
                        }
                    }
                }
                else
                {
                    try
                    {
                        await JobberConfiguration.SendEndpoints[job.QueueName].Send(job.Job);

                        producedJobs++;
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add(job.Job.CorrelationId, JsonConvert.SerializeObject(job.Job.Data));

                        Logger.Error(ex);
                    }
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