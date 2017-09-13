using System;
using Autofac;
using Jobber.Core;

namespace Jobber.Sample.StandaloneJobWorkerWithAutofac
{
    class Program
    {
        static void Main(string[] args)
        {
            var containerBuilder = new Autofac.ContainerBuilder();
            containerBuilder.RegisterType<TodoStandaloneJobWorker>().AsSelf().InstancePerLifetimeScope();
            var container = containerBuilder.Build();

            string jobName = "Todo";
            int restartDelayInMinutes = 1;
            TimeSpan schedulingTickTime = TimeSpan.FromMilliseconds(1);

            JobberBuilder.Instance.SetJobName(jobName)
                                  .EnableServiceRecovery(restartDelayInMinutes)
                                  .CreateStandaloneJobWorker()
                                        .SetStandaloneJobWorker<TodoStandaloneJobWorker>()
                                        .SetJobScope(new JobScope(container))
                                        .SetSchedulingTickTime(schedulingTickTime)
                                        .RunAsLocalService();
        }
    }
}