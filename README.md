#   **Jobber**
------------------------------

![alt tag](https://raw.githubusercontent.com/GokGokalp/Jobber/develop/misc/jobber-logo.png)

Jobber is lightweight, simple and distributed task scheduler.

[![NuGet version](https://badge.fury.io/nu/Jobber.svg)](https://badge.fury.io/nu/Jobber)

### NuGet Packages
``` 
PM> Install-Package Jobber
```

####Features:
- Easy to use
- Pub/Sub mode distributed task scheduling
- Standalone mode task scheduling
- Includes high availability modes for producers (ActiveActive-ActivePassive)
- Includes service recovery modes
- Logging (currently only support NLog)


####To-Do:
- Dashboard for service instances
- Abstraction for logging

Usage:
-----
For **standalone** job worker mode:

```cs
class TodoStandaloneJobWorker : StandaloneJobWorkerBase
{
    protected override void ExecuteJob()
    {
        Console.WriteLine("Hello World!");
    }
}
```

just inherit the _StandaloneJobWorkerBase_ for your job worker class, and then initialize as follows:

```cs
class Program
{
    static void Main(string[] args)
    {
        string jobName = "Todo";
        int restartDelayInMinutes = 1;
        TimeSpan schedulingTickTime = TimeSpan.FromSeconds(5);
        TimeSpan lockDuration = TimeSpan.FromSeconds(10);
        List<EndPoint> redisEndPoints = new List<EndPoint>()
            {
                new DnsEndPoint("", 6379)
            };

        JobberBuilder.Instance.SetJobName(jobName)
                              .EnableServiceRecovery(restartDelayInMinutes)
                              .CreateStandaloneJobWorker()
                                    .SetStandaloneJobWorker<TodoStandaloneJobWorker>()
                                    .SetSchedulingTickTime(schedulingTickTime)
                                    .HighAvailabilitySetup()
                                            .UseActivePassive()
                                            .InitializeRedisForLocking(redisEndPoints)
                                            .SetLockingDuration(lockDuration)
                                            .Then()
                                    .RunAsLocalService();
    }
}
```

For **pub/sub** job worker mode, firstly let's initialize job producer.

```cs
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
```

just inherit the _JobProducerBase<TJob>_. TJob is your job entity. After this, override the _GetJobs()_ method and return your job data. Now initialize producer as follows:

```cs
class Program
{
    static void Main(string[] args)
    {
        string jobName = "Todo";
        string rabbitMqUri = "";
        string rabbitMqUserName = "";
        string rabbitMqPassword = "";
        string rabbitMqTodoQueueName = "todo.queue";
        int restartDelayInMinutes = 1;
        TimeSpan schedulingTickTime = TimeSpan.FromSeconds(5);
        TimeSpan lockDuration = TimeSpan.FromSeconds(10);
        List<EndPoint> redisEndPoints = new List<EndPoint>()
            {
                new DnsEndPoint("", 6379)
            };

        JobberBuilder.Instance.SetJobName(jobName)
                              .EnableServiceRecovery(restartDelayInMinutes)
                              .CreateJobProducer()
                                    .SetRabbitMqCredentials(rabbitMqUri, rabbitMqUserName, rabbitMqPassword)
                                    .SetQueueName(rabbitMqTodoQueueName)
                                    .SetJobProducer<TodoJobProducer>()
                                    .SetSchedulingTickTime(schedulingTickTime)
                                    .HighAvailabilitySetup()
                                            .UseActivePassive()
                                            .InitializeRedisForLocking(redisEndPoints)
                                            .SetLockingDuration(lockDuration)
                                            .Then()
                                    .RunAsLocalService();

    }
}
```

For job consumer:

```cs
class TodoJobConsumer : JobConsumerBase<Todo>
{
    public override async Task ConsumeJob(ConsumeContext<IJob<Todo>> job)
    {
        await Console.Out.WriteLineAsync(job.Message.Data.TodoNumber);
    }
}
```

then initialize job consumer as follow:

```cs
class Program
{
    static void Main(string[] args)
    {
        string jobName = "Todo";
        string rabbitMqUri = "";
        string rabbitMqUserName = "";
        string rabbitMqPassword = "";
        string rabbitMqTodoQueueName = "todo.queue";
        int incrementalRetryLimit = 3;
        TimeSpan initialIncrementalRetryInterval = TimeSpan.FromMinutes(5);
        TimeSpan intervalIncrementalRetryIncrement = TimeSpan.FromMinutes(10);
        int restartDelayInMinutes = 1;

        JobberBuilder.Instance.SetJobName(jobName)
                              .EnableServiceRecovery(restartDelayInMinutes)
                              .CreateJobConsumer()
                                    .SetRabbitMqCredentials(rabbitMqUri, rabbitMqUserName, rabbitMqPassword)
                                    .UseIncrementalRetryPolicy(incrementalRetryLimit, initialIncrementalRetryInterval, intervalIncrementalRetryIncrement)
                                    .SetQueueName(rabbitMqTodoQueueName)
                                    .SetJobConsumer<TodoJobConsumer>()
                                    .RunAsLocalService();
    }
}
```

####Samples:
- [Jobber.Sample.JobConsumer]
- [Jobber.Sample.JobConsumerWithAutofac]
- [Jobber.Sample.JobProducer]
- [Jobber.Sample.StandaloneJobWorker]
- [Jobber.Sample.StandaloneJobWorkerWithAutofac]

[Jobber.Sample.JobConsumer]: https://github.com/GokGokalp/Jobber/tree/develop/Jobber.Sample.JobConsumer
[Jobber.Sample.JobConsumerWithAutofac]: https://github.com/GokGokalp/Jobber/tree/develop/Jobber.Sample.JobConsumerWithAutofac
[Jobber.Sample.JobProducer]: https://github.com/GokGokalp/Jobber/tree/develop/Jobber.Sample.JobProducer
[Jobber.Sample.StandaloneJobWorker]: https://github.com/GokGokalp/Jobber/tree/develop/Jobber.Sample.StandaloneJobWorker
[Jobber.Sample.StandaloneJobWorkerWithAutofac]: https://github.com/GokGokalp/Jobber/tree/develop/Jobber.Sample.StandaloneJobWorkerWithAutofac