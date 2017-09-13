using NLog;

namespace Jobber.Core
{
    public abstract class StandaloneJobWorkerBase : IStandaloneJobWorker
    {
        private static readonly Logger Logger = LogManager.GetLogger(JobberConfiguration.JobName);

        protected abstract void ExecuteJob();

        public void Execute()
        {
            Logger.Info("[StandaloneJobWorkerBase.Execute] method begin.");

            ExecuteJob();

            Logger.Info("[StandaloneJobWorkerBase.Execute] method end.");
        }
    }
}