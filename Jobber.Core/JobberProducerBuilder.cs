using System;
using System.Timers;
using MassTransit;
using NLog;

namespace Jobber.Core
{
    public class JobberProducerBuilder : JobberBuilderBase<JobberProducerBuilder>, IBuilder
    {
        private static readonly Lazy<JobberProducerBuilder> _Instance = new Lazy<JobberProducerBuilder>(() => new JobberProducerBuilder());

        private JobberProducerBuilder()
        {
        }

        public static JobberProducerBuilder Instance => _Instance.Value;

        private static Logger _logger;
        private static Type _jobType;
        private static IJobScope _jobScope = new DefaultJobScope();
        private static Timer _timer;
        private static string _rabbitMqUri;
        private static string _rabbitMqUserName;
        private static string _rabbitMqPassword;
        private static IBusControl _bus;

        #region Fluent Methods
        public JobberProducerModeBuilder HighAvailabilitySetup()
        {
           return JobberProducerModeBuilder.Instance;
        }

        public JobberProducerBuilder SetSchedulingTickTime(TimeSpan tickTime)
        {
            JobberConfiguration.TickTime = tickTime;

            _timer = new Timer { Interval = tickTime.TotalMilliseconds };
            _timer.Elapsed += OnTimedEvent;

            return this;
        }

        public JobberProducerBuilder SetRabbitMqCredentials(string rabbitMqUri, string rabbitMqUserName, string rabbitMqPassword)
        {
            _rabbitMqUri = rabbitMqUri;
            _rabbitMqUserName = rabbitMqUserName;
            _rabbitMqPassword = rabbitMqPassword;

            return this;
        }

        public JobberProducerBuilder SetMultipleQueueName(params string[] queueNames)
        {
            _bus = CreateBus();

            foreach (var queueName in queueNames)
            {
                string _queueName = queueName;
                if (!_rabbitMqUri.EndsWith("/"))
                {
                    _queueName = _queueName.Insert(0, "/");
                }

                var sendToUri = new Uri($"{_rabbitMqUri}{_queueName}");

                JobberConfiguration.SendEndpoints.Add(_queueName, _bus.GetSendEndpoint(sendToUri).Result);
            }

            return this;
        }

        public JobberProducerBuilder SetQueueName(string queueName)
        {
            SetMultipleQueueName(queueName);

            return this;
        }

        public JobberProducerBuilder SetJobProducer<TJobProducer>() where TJobProducer : IJobProducer
        {
            _jobType = typeof(TJobProducer);

            return this;
        }

        public JobberProducerBuilder SetJobScope(IJobScope instance)
        {
            _jobScope = instance;

            return this;
        }

        public void RunAsLocalService()
        {
            base.RunAsLocalService(service => Instance, $"{JobberConfiguration.JobName}.Producer");
        }

        public void RunAsNetworkService()
        {
            base.RunAsNetworkService(service => Instance, $"{JobberConfiguration.JobName}.Producer");
        }

        public void RunAsService(string username, string password)
        {
            base.RunAsService(service => Instance, $"{JobberConfiguration.JobName}.Producer", username, password);
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
        private IBusControl CreateBus()
        {
            _bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(_rabbitMqUri), hst =>
                {
                    hst.Username(_rabbitMqUserName);
                    hst.Password(_rabbitMqPassword);
                });
            });

            return _bus;
        }

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
                        IJobProducer producer = (IJobProducer)scope.CreateJobInstance(_jobType);

                        producer.ProduceJobs();
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