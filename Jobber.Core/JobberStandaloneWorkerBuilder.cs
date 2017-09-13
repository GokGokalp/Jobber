using System;
using System.Timers;
using NLog;

namespace Jobber.Core
{
    public class JobberStandaloneWorkerBuilder : JobberBuilderBase<JobberStandaloneWorkerBuilder>, IBuilder
    {
        private static readonly Lazy<JobberStandaloneWorkerBuilder> _Instance =
            new Lazy<JobberStandaloneWorkerBuilder>(() => new JobberStandaloneWorkerBuilder());

        private JobberStandaloneWorkerBuilder()
        {
        }

        public static JobberStandaloneWorkerBuilder Instance => _Instance.Value;

        private static Logger _logger;
        private static Timer _timer;
        private static Type _jobType;
        private static IJobScope _jobScope = new DefaultJobScope();

        #region Fluent Methods
        public JobberStandaloneWorkerModeBuilder HighAvailabilitySetup()
        {
            return JobberStandaloneWorkerModeBuilder.Instance;
        }

        public JobberStandaloneWorkerBuilder SetSchedulingTickTime(TimeSpan tickTime)
        {
            JobberConfiguration.TickTime = tickTime;

            _timer = new Timer { Interval = tickTime.TotalMilliseconds };
            _timer.Elapsed += OnTimedEvent;

            return this;
        }

        public JobberStandaloneWorkerBuilder SetStandaloneJobWorker<TStandaloneJobWorker>() where TStandaloneJobWorker : IStandaloneJobWorker
        {
            _jobType = typeof(TStandaloneJobWorker);

            return this;
        }

        public JobberStandaloneWorkerBuilder SetJobScope(IJobScope instance)
        {
            _jobScope = instance;

            return this;
        }

        public void RunAsLocalService()
        {
            base.RunAsLocalService(service => Instance, $"{JobberConfiguration.JobName}.StandaloneWorker");
        }

        public void RunAsNetworkService()
        {
            base.RunAsNetworkService(service => Instance, $"{JobberConfiguration.JobName}.StandaloneWorker");
        }

        public void RunAsService(string username, string password)
        {
            base.RunAsService(service => Instance, $"{JobberConfiguration.JobName}.StandaloneWorker", username, password);
        }
        #endregion

        #region Public Methods
        public void Start()
        {
            if (_logger == null)
            {
                _logger = LogManager.GetLogger(JobberConfiguration.JobName);
            }

            _timer.Start();

            _logger.Info($"[{JobberConfiguration.JobName}] job service started. Start time: {DateTime.Now}");
        }

        public void Stop()
        {
            _timer.Stop();

            _logger.Info($"[{JobberConfiguration.JobName}] job service stopped. Stop time: {DateTime.Now}");
        }
        #endregion

        #region Private Methods
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            ((Timer)source).Stop();

            System.Diagnostics.Trace.CorrelationManager.ActivityId = Guid.NewGuid();

            JobberSupervisor.Instance.KeepOn(JobberConfiguration.JobName, JobberConfiguration.LockingDuration, () =>
            {
                _logger.Info($"[{JobberConfiguration.JobName}] job [OnTimedEvent] started. Start time: {DateTime.Now}");

                try
                {
                    using (var scope = _jobScope.CreateJobScope())
                    {
                        IStandaloneJobWorker worker = (IStandaloneJobWorker)scope.CreateJobInstance(_jobType);

                        worker.Execute();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

                _logger.Info($"[{JobberConfiguration.JobName}] job [OnTimedEvent] ended. End time: {DateTime.Now}");
            });

            ((Timer)source).Start();
        }
        #endregion
    }
}