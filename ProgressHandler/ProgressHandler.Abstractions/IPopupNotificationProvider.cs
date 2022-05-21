namespace ProgressHandler.Abstractions
{
    /// <summary>
    /// Type provides methods to create popup user notifications.
    /// </summary>
    public interface IPopupNotificationProvider
    {
        void CreateProgressToast(String step, UInt32 totalIterations);
        void UpdateProgress(Single progress, UInt32 iterations);

        void UpdateProgressStep(String step);
    }
}
