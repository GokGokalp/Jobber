using System;
using Jobber.Core;

namespace Jobber.Sample.JobConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
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
                                        .SetJobConsumer<TodoJobConsumer>()
                                        .RunAsLocalService();
        }
    }
}