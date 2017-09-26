using System;
using System.Collections.Generic;
using System.Net;
using Jobber.Core;

namespace Jobber.Sample.StandaloneJobWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            string jobName = "Todo";
            int restartDelayInMinutes = 1;
            TimeSpan schedulingTickTime = TimeSpan.FromSeconds(5);
            TimeSpan lockDuration = TimeSpan.FromSeconds(10);
            List<EndPoint> redisEndPoints = new List<EndPoint>()
            {
                new DnsEndPoint("", 6379)
            };
            string redisPassword = "sayHello";

            JobberBuilder.Instance.SetJobName(jobName)
                                  .EnableServiceRecovery(restartDelayInMinutes)
                                  .CreateStandaloneJobWorker()
                                        .SetStandaloneJobWorker<TodoStandaloneJobWorker>()
                                        .SetSchedulingTickTime(schedulingTickTime)
                                        .HighAvailabilitySetup()
                                                .UseActivePassive()
                                                .InitializeRedisForLocking(redisEndPoints, redisPassword)
                                                .SetLockingDuration(lockDuration)
                                                .Then()
                                        .RunAsLocalService();
        }
    }
}