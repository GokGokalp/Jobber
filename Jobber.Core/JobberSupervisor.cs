using System;

namespace Jobber.Core
{
    class JobberSupervisor
    {
        private static readonly Lazy<JobberSupervisor> _Instance = new Lazy<JobberSupervisor>(() => new JobberSupervisor());
        private readonly ILockManager _lockManager;
        private string _lockedDate;

        private JobberSupervisor()
        {
            if (JobberConfiguration.ProducerMode == ProducerMode.ActivePassive)
            {
                _lockManager = RedisLockManager.Instance.Initialize(JobberConfiguration.RedisEndPoints.ToArray(), JobberConfiguration.RedisPassword, JobberConfiguration.RedisDatabase);
            } 
        }

        public static JobberSupervisor Instance => _Instance.Value;

        public void KeepOn(string jobName, TimeSpan lockingDuration, Action action)
        {
            if (JobberConfiguration.ProducerMode == ProducerMode.ActiveActive)
            {
                action.Invoke();
            }
            else if (JobberConfiguration.ProducerMode == ProducerMode.ActivePassive)
            {
                bool isLocked = IsLocked(jobName);

                if (!isLocked)
                {
                    bool getLock = GetLock(jobName, lockingDuration);

                    if (getLock)
                    {
                        action.Invoke();

                        ExtendLock(jobName, lockingDuration);
                    }
                }
                else
                {
                    bool isLockedForThisInstance = IsLockedForThisInstance(jobName, _lockedDate);

                    if (isLockedForThisInstance)
                    {
                        ExtendLock(jobName, lockingDuration);

                        action.Invoke();

                        ExtendLock(jobName, lockingDuration);
                    }
                }
            }
        }

        public bool IsLocked(string jobName)
        {
            bool locked = _lockManager.IsLocked(jobName);

            return locked;
        }

        public bool IsLockedForThisInstance(string jobName, string lockedDate)
        {
            bool locked = _lockManager.IsLockedForThisInstance(jobName, lockedDate);

            return locked;
        }

        public bool GetLock(string jobName, TimeSpan expiryTime)
        {
            _lockedDate = DateTime.Now.ToShortTimeString();

            bool takeLock = _lockManager.GetLock(jobName, _lockedDate, expiryTime);

            return takeLock;
        }

        public void ExtendLock(string jobName, TimeSpan expiryTime)
        {
            _lockManager.ExtendLock(jobName, _lockedDate, expiryTime);
        }
    }
}