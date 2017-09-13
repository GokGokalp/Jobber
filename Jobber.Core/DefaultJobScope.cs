using System;

namespace Jobber.Core
{
    internal class DefaultJobScope : IJobScope
    {
        public IJobScope CreateJobScope()
        {
            return new DefaultJobScope();
        }

        public TType CreateJobInstance<TType>()
        {
            return Activator.CreateInstance<TType>();
        }

        public object CreateJobInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public void Dispose()
        {
        }
    }
}