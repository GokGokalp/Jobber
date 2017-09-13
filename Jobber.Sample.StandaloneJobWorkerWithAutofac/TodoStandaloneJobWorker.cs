using System;
using Jobber.Core;

namespace Jobber.Sample.StandaloneJobWorkerWithAutofac
{
    class TodoStandaloneJobWorker : StandaloneJobWorkerBase
    {
        public TodoStandaloneJobWorker()
        {
            
        }

        protected override void ExecuteJob()
        {
            Console.WriteLine("Hello World!");
        }
    }
}