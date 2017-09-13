using System;

namespace Jobber.Core
{
    public interface IJob<TData>
    {
        Guid CorrelationId { get; }
        TData Data { get; set; }
    }
}