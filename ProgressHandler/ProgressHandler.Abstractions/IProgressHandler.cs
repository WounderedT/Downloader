using System.ComponentModel;

namespace ProgressHandler.Abstractions
{
    /// <summary>
    /// Type provides methods to track progress of long-running tasks.
    /// </summary>
    public interface IProgressHandler : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets progress step text.
        /// </summary>
        String ProgressStepText { get; set; }

        /// <summary>
        /// Gets or sets progress value as string.
        /// </summary>
        String ProgressValueText { get; set; }

        /// <summary>
        /// Starts new progress tracking.
        /// </summary>
        /// <param name="step">Step name.</param>
        /// <param name="iterationsCount">Total number of iterations to track progress of.</param>
        void Start(String step, UInt32 iterationsCount);

        /// <summary>
        /// Sets name of the current step.
        /// </summary>
        /// <param name="name">Step name.</param>
        void Step(String name);

        /// <summary>
        /// Sets progress value to 100% and stops progress tracking.
        /// </summary>
        void Stop();

        /// <summary>
        /// Updates total progress by pre-calculated iteraction value.
        /// </summary>
        void Update();

        /// <summary>
        /// Updates total progress by <paramref name="progress"/> value
        /// </summary>
        /// <param name="progress">Value to update total progress by.</param>
        void Update(UInt32 progress);
    }
}
