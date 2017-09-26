using System;
using System.Collections.Generic;
using System.Net;

namespace Jobber.Core
{
    public class JobberProducerModeBuilder
    {
        private static readonly Lazy<JobberProducerModeBuilder> _Instance =
            new Lazy<JobberProducerModeBuilder>(() => new JobberProducerModeBuilder());

        private JobberProducerModeBuilder()
        {
        }

        public static JobberProducerModeBuilder Instance => _Instance.Value;

        #region Fluent Methods

        public JobberProducerModeBuilder UseActiveActive()
        {
            JobberConfiguration.ProducerMode = ProducerMode.ActiveActive;

            return this;
        }

        public JobberProducerActivePassiveModeBuilder UseActivePassive()
        {
            JobberConfiguration.ProducerMode = ProducerMode.ActivePassive;

            return JobberProducerActivePassiveModeBuilder.Instance;
        }

        public JobberProducerBuilder Then()
        {
            return JobberProducerBuilder.Instance;
        }

        #endregion
    }

    public class JobberProducerActivePassiveModeBuilder
    {
        private static readonly Lazy<JobberProducerActivePassiveModeBuilder> _Instance =
    new Lazy<JobberProducerActivePassiveModeBuilder>(() => new JobberProducerActivePassiveModeBuilder());

        private JobberProducerActivePassiveModeBuilder()
        {
        }

        public static JobberProducerActivePassiveModeBuilder Instance => _Instance.Value;

        public JobberProducerActivePassiveModeBuilder InitializeRedisForLocking(List<EndPoint> redisEndPoints, string password = null)
        {
            JobberConfiguration.RedisEndPoints = redisEndPoints;
            JobberConfiguration.RedisPassword = password;

            return this;
        }

        public JobberProducerActivePassiveModeBuilder SetLockingDuration(TimeSpan lockingDuration)
        {
            JobberConfiguration.LockingDuration = lockingDuration;

            return this;
        }

        public JobberProducerBuilder Then()
        {
            return JobberProducerBuilder.Instance;
        }
    }
}