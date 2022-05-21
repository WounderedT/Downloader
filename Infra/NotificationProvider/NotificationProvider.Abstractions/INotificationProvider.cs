namespace NotificationProvider.Abstractions
{
    public interface INotificationProvider
    {
        void PopErrorToast(String message);

        void PopInfoToast(String message, String downloadPath);
    }
}
