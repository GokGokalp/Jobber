using System;
using System.Threading.Tasks;
using Jobber.Core;
using Jobber.Sample.Contract;
using MassTransit;

namespace Jobber.Sample.JobConsumerWithAutofac
{
    class TodoJobConsumer : JobConsumerBase<Todo>
    {
        public override async Task ConsumeJob(ConsumeContext<IJob<Todo>> job)
        {
            await Console.Out.WriteLineAsync(job.Message.Data.TodoNumber);
        }
    }
}