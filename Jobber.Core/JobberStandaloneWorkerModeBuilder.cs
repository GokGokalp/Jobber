using System;
using System.Collections.Generic;
using System.Net;

namespace Jobber.Core
{
    public class JobberStandaloneWorkerModeBuilder
    {
        private static readonly Lazy<JobberStandaloneWorkerModeBuilder> _Instance =
            new Lazy<JobberStandaloneWorkerModeBuilder>(() => new JobberStandaloneWorkerModeBuilder());

        private JobberStandaloneWorkerModeBuilder()
        {
        }

        public static JobberStandaloneWorkerModeBuilder Instance => _Instance.Value;

        #region Fluent Methods

        public JobberStandaloneWorkerModeBuilder UseActiveActive()
        {
            JobberConfiguration.ProducerMode = ProducerMode.ActiveActive;

            return this;
        }

        public JobberStandaloneWorkerActivePassiveModeBuilder UseActivePassive()
        {
            JobberConfiguration.ProducerMode = ProducerMode.ActivePassive;

            return JobberStandaloneWorkerActivePassiveModeBuilder.Instance;
        }

        public JobberStandaloneWorkerBuilder Then()
        {
            return JobberStandaloneWorkerBuilder.Instance;
        }

        #endregion
    }

    public class JobberStandaloneWorkerActivePassiveModeBuilder
    {
        private static readonly Lazy<JobberStandaloneWorkerActivePassiveModeBuilder> _Instance =
    new Lazy<JobberStandaloneWorkerActivePassiveModeBuilder>(() => new JobberStandaloneWorkerActivePassiveModeBuilder());

        private JobberStandaloneWorkerActivePassiveModeBuilder()
        {
        }

        public static JobberStandaloneWorkerActivePassiveModeBuilder Instance => _Instance.Value;

        public JobberStandaloneWorkerActivePassiveModeBuilder InitializeRedisForLocking(List<EndPoint> redisEndPoints, string password = null)
        {
            JobberConfiguration.RedisEndPoints = redisEndPoints;
            JobberConfiguration.RedisPassword = password;

            return this;
        }

        public JobberStandaloneWorkerActivePassiveModeBuilder SetLockingDuration(TimeSpan lockingDuration)
        {
            JobberConfiguration.LockingDuration = lockingDuration;

            return this;
        }

        public JobberStandaloneWorkerBuilder Then()
        {
            return JobberStandaloneWorkerBuilder.Instance;
        }
    }
}