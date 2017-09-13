using System;
using Jobber.Core;

namespace Jobber.Sample.StandaloneJobWorker
{
    class TodoStandaloneJobWorker :  StandaloneJobWorkerBase
    {
        protected override void ExecuteJob()
        {
            Console.WriteLine("Hello World!");
        }
    }
}