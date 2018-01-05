namespace Jobber.Core
{
    public class MultipleQueueJob<TJob>
    {
        public string QueueName { get; set; }
        public IJob<TJob> Job { get; set; }
    }
}