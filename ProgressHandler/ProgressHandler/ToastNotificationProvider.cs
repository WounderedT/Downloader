using Microsoft.Toolkit.Uwp.Notifications;
using ProgressHandler.Abstractions;
using Windows.UI.Notifications;

namespace ProgressHandler
{
    internal class ToastNotificationProvider : IPopupNotificationProvider
    {
        // Define a tag (and optionally a group) to uniquely identify the notification, in order update the notification data later;
        private const String Tag = "progressToast";
        private const String Group = "downloads";

        private UInt32 _sequenceNumber = 0;
        private UInt32 _totalIterations;
        private Boolean _skipUpdate = false;

        public void CreateProgressToast(String step, UInt32 totalIterations)
        {
            _sequenceNumber = 1;
            _totalIterations = totalIterations;
            _skipUpdate = false;

            // Construct the toast content with data bound fields
            ToastContent? content = new ToastContentBuilder()
                .AddText("Downloading...")
                .AddVisualChild(new AdaptiveProgressBar()
                {
                    Title = "",
                    Value = new BindableProgressBarValue("progressValue"),
                    ValueStringOverride = new BindableString("progressValueString"),
                    Status = new BindableString("progressStatus")
                })
                .GetToastContent();

            // Generate the toast notification
            var toast = new ToastNotification(content.GetXml())
            {

                // Assign the tag and group
                Tag = Tag,
                Group = Group,

                // Assign initial NotificationData values
                // Values must be of type string
                Data = new NotificationData()
            };
            toast.Data.Values["progressValue"] = "0.0";
            toast.Data.Values["progressValueString"] = $"0/{_totalIterations}";
            toast.Data.Values["progressStatus"] = step;

            // Provide sequence number to prevent out-of-order updates, or assign 0 to indicate "always update"
            toast.Data.SequenceNumber = _sequenceNumber;

            // Show the toast notification to the user
            //var toastNotification = ToastContentBuilder.CreateProgressBarData(content, title: "", value: 0.0, valueStringOverride: $"0/{_totalIterations}", status: step, sequence: _sequenceNumber);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public void UpdateProgress(Single progress, UInt32 iterations)
        {
            if (_skipUpdate)
            {
                return;
            }

            // Create NotificationData and make sure the sequence number is incremented
            // since last update, or assign 0 for updating regardless of order
            var data = new NotificationData
            {
                SequenceNumber = ++_sequenceNumber
            };

            // Assign new values
            data.Values["progressValue"] = Math.Round(progress / 100, 3).ToString();
            data.Values["progressValueString"] = $"{iterations}/{_totalIterations}";

            // Update the existing notification's data by using tag/group
            NotificationUpdateResult updateResult = ToastNotificationManager.CreateToastNotifier().Update(data, Tag, Group);
            _skipUpdate = updateResult != NotificationUpdateResult.Succeeded;
        }

        public void UpdateProgressStep(String step)
        {
            if (_skipUpdate)
            {
                return;
            }

            var data = new NotificationData
            {
                SequenceNumber = ++_sequenceNumber
            };

            data.Values["progressStatus"] = step;

            NotificationUpdateResult updateResult = ToastNotificationManager.CreateToastNotifier().Update(data, Tag, Group);
            _skipUpdate = updateResult != NotificationUpdateResult.Succeeded;
        }
    }
}
