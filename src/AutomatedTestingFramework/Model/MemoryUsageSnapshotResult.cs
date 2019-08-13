namespace AutomatedTestingFramework.Model
{
    public class MemoryUsageSnapshotResult : StepResult
    {
        #region Public Properties
        public long? MemoryUsage { get; set; }
        #endregion

        #region Public Properties
        public MemoryUsageSnapshotResult()
        {
        }

        public MemoryUsageSnapshotResult(StepBase step, long? memoryUsage) : base(step)
        {
            MemoryUsage = memoryUsage;
        }
        #endregion
    }
}
