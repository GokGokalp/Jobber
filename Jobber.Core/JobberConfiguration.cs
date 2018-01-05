using System;
using System.Collections.Generic;
using System.Net;
using MassTransit;

namespace Jobber.Core
{
    internal enum ProducerMode
    {
        ActiveActive = 0,
        ActivePassive = 1
    }

    class JobberConfiguration
    {
        public static string JobName { get; set; }
        public static ProducerMode ProducerMode { get; set; }
        public static TimeSpan TickTime { get; set; }
        public static bool EnableServiceRecovery { get; set; }
        public static int RestartDelayInMinutes { get; set; }
        public static ISendEndpoint SendEndpoint { get; set; }
        public static Dictionary<string, ISendEndpoint> SendEndpoints = new Dictionary<string, ISendEndpoint>();
        public static List<EndPoint> RedisEndPoints { get; set; }
        public static string RedisPassword { get; set; }
        public static TimeSpan LockingDuration { get; set; }
        public static int RedisDatabase { get; set; }
    }
}