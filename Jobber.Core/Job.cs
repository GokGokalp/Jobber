using System;

namespace Jobber.Core
{
    public class Job<TData> : IJob<TData>
    {
        public Guid CorrelationId => System.Diagnostics.Trace.CorrelationManager.ActivityId;

        public TData Data { get; set; }
    }
}