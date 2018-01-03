using System;
using Autofac;
using GreenPipes;
using MassTransit;
using MassTransit.RabbitMqTransport;
using NLog;

namespace Jobber.Core
{
    public class JobberConsumerBuilder : JobberBuilderBase<JobberConsumerBuilder>, IBuilder
    {
        private static readonly Lazy<JobberConsumerBuilder> _Instance = new Lazy<JobberConsumerBuilder>(() => new JobberConsumerBuilder());

        private JobberConsumerBuilder()
        {
        }

        public static JobberConsumerBuilder Instance => _Instance.Value;

        private static Logger _logger;
        private static string _rabbitMqUri;
        private static string _rabbitMqUserName;
        private static string _rabbitMqPassword;
        private static string _queueName;
        private static IBusControl _bus;
        private bool _useRetry;
        private int _incrementalRetryLimit;
        private TimeSpan _initialIncrementalRetryInterval;
        private TimeSpan _intervalIncrementalRetryIncrement;
        private int? _concurrencyLimit;
        private int? _rateLimit;
        private TimeSpan? _rateLimitInterval;

        #region Fluent Methods
        public JobberConsumerBuilder SetRabbitMqCredentials(string rabbitMqUri, string rabbitMqUserName, string rabbitMqPassword)
        {
            _rabbitMqUri = rabbitMqUri;
            _rabbitMqUserName = rabbitMqUserName;
            _rabbitMqPassword = rabbitMqPassword;

            return this;
        }

        public JobberConsumerBuilder SetQueueName(string queueName)
        {
            _queueName = queueName;

            return this;
        }

        public JobberConsumerBuilder SetConcurrencyLimit(int concurrencyLimit)
        {
            _concurrencyLimit = concurrencyLimit;

            return this;
        }

        public JobberConsumerBuilder SetRateLimit(int rateLimit, TimeSpan interval)
        {
            _rateLimit = rateLimit;
            _rateLimitInterval = interval;

            return this;
        }

        public JobberConsumerBuilder UseIncrementalRetryPolicy(int incrementalRetryLimit, TimeSpan initialIncrementalRetryInterval, TimeSpan intervalIncrementalRetryIncrement)
        {
            _useRetry = true;
            _incrementalRetryLimit = incrementalRetryLimit;
            _initialIncrementalRetryInterval = initialIncrementalRetryInterval;
            _intervalIncrementalRetryIncrement = intervalIncrementalRetryIncrement;

            return this;
        }

        public JobberConsumerBuilder SetJobConsumer<TJobConsumer>() where TJobConsumer : class, IConsumer, new()
        {
            _bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(_rabbitMqUri), hst =>
                {
                    hst.Username(_rabbitMqUserName);
                    hst.Password(_rabbitMqPassword);
                    hst.Heartbeat(60);
                });

                UseIncrementalRetryPolicy(cfg);
                
                cfg.ReceiveEndpoint(host, _queueName, e =>
                {
                    SetRateLimit(e);
                    SetConcurrencyLimit(e);
                    e.Consumer<TJobConsumer>();
                });
            });

            return this;
        }

        public JobberConsumerBuilder SetJobConsumer(ILifetimeScope lifetimeScope)
        {
            _bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(_rabbitMqUri), hst =>
                {
                    hst.Username(_rabbitMqUserName);
                    hst.Password(_rabbitMqPassword);
                    hst.Heartbeat(60);
                });

                UseIncrementalRetryPolicy(cfg);

                cfg.ReceiveEndpoint(host, _queueName, e =>
                {
                    SetRateLimit(e);
                    SetConcurrencyLimit(e);
                    e.LoadFrom(lifetimeScope);
                });
            });

            return this;
        }

        public void RunAsLocalService()
        {
            base.RunAsLocalService(service => Instance, $"{JobberConfiguration.JobName}.Consumer");
        }

        public void RunAsNetworkService()
        {
            base.RunAsNetworkService(service => Instance, $"{JobberConfiguration.JobName}.Consumer");
        }

        public void RunAsService(string username, string password)
        {
            base.RunAsService(service => Instance, $"{JobberConfiguration.JobName}.Consumer", username, password);
        }
        #endregion

        #region Public Methods
        public void Start()
        {
            if (_logger == null)
            {
                _logger = LogManager.GetLogger(JobberConfiguration.JobName);
            }

            _bus.Start();

            _logger.Info($"[{JobberConfiguration.JobName}] job service started. Start time: {DateTime.Now}");
        }

        public void Stop()
        {
            _bus.Stop();

            _logger.Info($"[{JobberConfiguration.JobName}] job service stopped. Stop time: {DateTime.Now}");
        }
        #endregion

        #region Private Methods
        private void UseIncrementalRetryPolicy(IRabbitMqBusFactoryConfigurator cfg)
        {
            if (_useRetry)
            {
                cfg.UseRetry(retryConfig =>
                {
                    retryConfig.Incremental(_incrementalRetryLimit, _initialIncrementalRetryInterval, _intervalIncrementalRetryIncrement);
                });
            }
        }

        private void SetConcurrencyLimit(IRabbitMqReceiveEndpointConfigurator cfg)
        {
            if (_concurrencyLimit != null)
            {
                cfg.UseConcurrencyLimit(_concurrencyLimit.Value);
            }
        }

        private void SetRateLimit(IRabbitMqReceiveEndpointConfigurator cfg)
        {
            if (_rateLimit != null && _rateLimitInterval != null)
            {
                cfg.UseRateLimit(_rateLimit.Value, _rateLimitInterval.Value);
            }
        }
        #endregion
    }
}