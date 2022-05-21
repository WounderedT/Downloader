namespace Handlers.Configuration.Retry
{
    public enum ConcurrentProcessingScaleEnum
    {
        /// <summary>
        /// Default number of processors is used in every retry iteration.
        /// </summary>
        None,
        /// <summary>
        /// Number of processors is halved in every retry iteration util it reaches 1.
        /// </summary>
        Half,
        /// <summary>
        /// Number of processors is reduced by 1 in every retry iteration util it reaches 1.
        /// </summary>
        Linear
    }

    public class RetryPolicyEntryConfig
    {
        public Int32 MaxParallelProcessingCount { get; set; } = Environment.ProcessorCount;
        public UInt32 Retries { get; set; }
        public ConcurrentProcessingScaleEnum ConcurrentProcessingScale { get; set; } = ConcurrentProcessingScaleEnum.None;

        public static RetryPolicyEntryConfig Default
        {
            get
            {
                return new RetryPolicyEntryConfig
                {
                    Retries = 5,
                    ConcurrentProcessingScale = ConcurrentProcessingScaleEnum.None
                };
            }
        }

        public Int32 ScaleThreadsCount(Int32 threads)
        {
            return ConcurrentProcessingScale switch
            {
                ConcurrentProcessingScaleEnum.None => threads,
                ConcurrentProcessingScaleEnum.Half => HalveThreadCount(threads),
                ConcurrentProcessingScaleEnum.Linear => threads > 2 ? --threads : 1,
                _ => throw new ArgumentException()
            };
        }

        private Int32 HalveThreadCount(Int32 threads)
        {
            if (threads == 1)
            {
                return threads;
            }
            var count = threads / 2;
            return count > 0 ? count : 1;
        }
    }
}
