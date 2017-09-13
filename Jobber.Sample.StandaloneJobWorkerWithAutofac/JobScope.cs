using System;
using Autofac;
using Jobber.Core;

namespace Jobber.Sample.StandaloneJobWorkerWithAutofac
{
    class JobScope : IJobScope
    {
        private readonly ILifetimeScope _lifetimeScope;

        public JobScope(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public IJobScope CreateJobScope()
        {
            return new JobScope(_lifetimeScope.BeginLifetimeScope());
        }

        public TType CreateJobInstance<TType>()
        {
            return _lifetimeScope.Resolve<TType>();
        }

        public object CreateJobInstance(Type type)
        {
            return _lifetimeScope.Resolve(type);
        }

        public void Dispose()
        {
            _lifetimeScope.Dispose();
        }
    }
}