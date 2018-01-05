using Jobber.Core;
using Jobber.Sample.Contract;
using System;
using System.Collections.Generic;

namespace Jobber.Sample.JobProducer
{
    class TodoJobMultipleQueueProducer : JobMultipleProducerBase<Todo>
    {
        protected override List<MultipleQueueJob<Todo>> GetJobs()
        {
            List<MultipleQueueJob<Todo>> jobs = new List<MultipleQueueJob<Todo>>
            {
                new MultipleQueueJob<Todo>
                {
                    QueueName = "todo.queue",
                    Job = new TodoJob
                    {
                        Data = new Todo() {TodoId = 1, TodoNumber = Guid.NewGuid().ToString()}
                    }
                },
                new MultipleQueueJob<Todo>
                {
                    QueueName = "todo.queue.2",
                    Job = new TodoJob
                    {
                        Data = new Todo() {TodoId = 2, TodoNumber = Guid.NewGuid().ToString()}
                    }
                }
            };

            return jobs;
        }
    }
}