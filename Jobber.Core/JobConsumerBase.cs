using System.Threading.Tasks;
using MassTransit;
using NLog;

namespace Jobber.Core
{
    public abstract class JobConsumerBase<TData> : IConsumer<IJob<TData>>
    {
        private static readonly Logger Logger = LogManager.GetLogger(JobberConfiguration.JobName);

        public async Task Consume(ConsumeContext<IJob<TData>> context)
        {
            System.Diagnostics.Trace.CorrelationManager.ActivityId = context.Message.CorrelationId;

            Logger.Info("[ConsumeJob] method begin.");

            await ConsumeJob(context);

            Logger.Info("[ConsumeJob] method end.");
        }

        public abstract Task ConsumeJob(ConsumeContext<IJob<TData>> job);
    }
}