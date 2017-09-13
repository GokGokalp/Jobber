using System;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.Runtime;

namespace Jobber.Core
{
    public abstract class JobberBuilderBase<TBuilder> where TBuilder : class, IBuilder
    {
        internal void RunAsLocalService(ServiceFactory<TBuilder> factory, string jobName)
        {
            Action<HostConfigurator> action = configurator =>
            {
                configurator.RunAsLocalService();
            };

            Run(factory, jobName, action);
        }

        internal void RunAsNetworkService(ServiceFactory<TBuilder> factory, string jobName)
        {
            Action<HostConfigurator> action = configurator =>
            {
                configurator.RunAsNetworkService();
            };

            Run(factory, jobName, action);
        }

        internal void RunAsService(ServiceFactory<TBuilder> factory, string jobName, string username, string password)
        {
            Action<HostConfigurator> action = configurator =>
            {
                configurator.RunAs(username, password);
            };

            Run(factory, jobName, action);
        }

        private void Run(ServiceFactory<TBuilder> factory, string jobName, Action<HostConfigurator> action)
        {
            HostFactory.Run(configurator =>
            {
                configurator.Service<TBuilder>(s =>
               {
                     s.ConstructUsing(factory);
                     s.WhenStarted(service => service.Start());
                     s.WhenStopped(service => service.Stop());
                 });

                if (JobberConfiguration.EnableServiceRecovery)
                {
                    configurator.EnableServiceRecovery(recovery =>
                   {
                         recovery.RestartService(JobberConfiguration.RestartDelayInMinutes);
                         recovery.OnCrashOnly();
                     });
                }

                action.Invoke(configurator);

                configurator.StartAutomatically();
                configurator.EnableShutdown();

                configurator.SetDescription($"{jobName}.Jobber.Service Topshelf Host");
                configurator.SetDisplayName($"{jobName}.Jobber.Service");
                configurator.SetServiceName($"{jobName}.Jobber.Service");
            });
        }
    }
}