using System;
using System.Collections.Generic;
using System.Net;
using Jobber.Core;

namespace Jobber.Sample.JobProducer
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
            string rabbitMqTodoQueueName2 = "todo.queue.2";
            int restartDelayInMinutes = 1;
            TimeSpan schedulingTickTime = TimeSpan.FromSeconds(5);
            TimeSpan lockDuration = TimeSpan.FromSeconds(10);
            //List<EndPoint> redisEndPoints = new List<EndPoint>()
            //{
            //    new DnsEndPoint("", 6379)
            //};

            JobberBuilder.Instance.SetJobName(jobName)
                                  .EnableServiceRecovery(restartDelayInMinutes)
                                  .CreateJobProducer()
                                        .SetRabbitMqCredentials(rabbitMqUri, rabbitMqUserName, rabbitMqPassword)
                                        //.SetQueueName(rabbitMqTodoQueueName)
                                        //.SetJobProducer<TodoJobProducer>()
                                        .SetMultipleQueueName(rabbitMqTodoQueueName, rabbitMqTodoQueueName2)
                                        .SetJobProducer<TodoJobMultipleQueueProducer>()
                                        .SetSchedulingTickTime(schedulingTickTime)
                                        //.HighAvailabilitySetup()
                                        //        .UseActivePassive()
                                        //        .InitializeRedisForLocking(redisEndPoints)
                                        //        .SetLockingDuration(lockDuration)
                                        //        .Then()
                                        .RunAsLocalService();
        }
    }
}