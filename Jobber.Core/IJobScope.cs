using System;

namespace Jobber.Core
{
    public interface IJobScope : IDisposable
    {
        IJobScope CreateJobScope();
        TType CreateJobInstance<TType>();
        object CreateJobInstance(Type type);
    }
}