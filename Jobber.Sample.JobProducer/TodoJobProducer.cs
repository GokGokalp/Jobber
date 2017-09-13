using System;
using System.Collections.Generic;
using Jobber.Core;
using Jobber.Sample.Contract;

namespace Jobber.Sample.JobProducer
{
    class TodoJobProducer : JobProducerBase<Todo>
    {
        protected override List<IJob<Todo>> GetJobs()
        {
            List<IJob<Todo>> jobs = new List<IJob<Todo>>
            {
                new TodoJob
                {
                    Data = new Todo() {TodoId = 1, TodoNumber = Guid.NewGuid().ToString()}
                },
                new TodoJob
                {
                    Data = new Todo() {TodoId = 2, TodoNumber = Guid.NewGuid().ToString()}
                }
            };

            return jobs;
        }
    }
}