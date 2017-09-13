using System;
using Autofac;
using Jobber.Core;

namespace Jobber.Sample.JobConsumerWithAutofac
{
    class Program
    {
        static void Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<TodoJobConsumer>().AsSelf().InstancePerLifetimeScope();
            var container = containerBuilder.Build();

            string jobName = "Todo";
            string rabbitMqUri = "";
            string rabbitMqUserName = "";
            string rabbitMqPassword = "";
            string rabbitMqTodoQueueName = "todo.queue";
            int incrementalRetryLimit = 3;
            TimeSpan initialIncrementalRetryInterval = TimeSpan.FromMinutes(5);
            TimeSpan intervalIncrementalRetryIncrement = TimeSpan.FromMinutes(10);
            int restartDelayInMinutes = 1;

            JobberBuilder.Instance.SetJobName(jobName)
                .EnableServiceRecovery(restartDelayInMinutes)
                .CreateJobConsumer()
                    .SetRabbitMqCredentials(rabbitMqUri, rabbitMqUserName, rabbitMqPassword)
                    .UseIncrementalRetryPolicy(incrementalRetryLimit, initialIncrementalRetryInterval, intervalIncrementalRetryIncrement)
                    .SetQueueName(rabbitMqTodoQueueName)
                    .SetJobConsumer(container)
                    .RunAsLocalService();
        }
    }
}