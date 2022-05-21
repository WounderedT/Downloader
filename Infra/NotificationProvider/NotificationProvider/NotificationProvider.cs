using Microsoft.Toolkit.Uwp.Notifications;
using NotificationProvider.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationProvider
{
    internal class NotificationProvider : INotificationProvider
    {
        private const String ActionArgumentName = "action";
        private const String ArgArgumentName = "arg";
        private const String ViewDestinationFolderDelegateName = "viewDestinationFolder";
        private const String OpenLogFileDelegateName = "openLogFile";

        private readonly DirectoryInfo _logDirectoryInfo;

        public NotificationProvider()
        {
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            //if (!Path.IsPathRooted(logFilePath))
            //{
            //    logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFilePath);
            //}
            _logDirectoryInfo = Directory.GetParent(logFilePath);
            if (_logDirectoryInfo == null)
            {
                throw new ArgumentException(nameof(logFilePath));
            }
            ToastNotificationManagerCompat.OnActivated += ToastNotificationOnActivatedHanlder;
        }

        ~NotificationProvider()
        {
            ToastNotificationManagerCompat.OnActivated -= ToastNotificationOnActivatedHanlder;
        }

        public void PopErrorToast(String message)
        {
            // Generate the toast notification content and pop the toast
            new ToastContentBuilder()
                .SetToastScenario(ToastScenario.Default)
                .AddText("Download failed")
                .AddText(message)
                .AddButton(new ToastButton()
                    .SetContent("View log")
                    .AddArgument(ActionArgumentName, OpenLogFileDelegateName)
                    .AddArgument(ArgArgumentName, ""))
                .AddButton(new ToastButton()
                    .SetDismissActivation())
                .Show();
        }

        public void PopInfoToast(String message, String downloadPath)
        {
            // Generate the toast notification content and pop the toast
            new ToastContentBuilder()
                .SetToastScenario(ToastScenario.Default)
                .AddText(message)
                .AddButton(new ToastButton()
                    .SetContent("View")
                    .AddArgument(ActionArgumentName, ViewDestinationFolderDelegateName)
                    .AddArgument(ArgArgumentName, downloadPath))
                .AddButton(new ToastButton()
                    .SetDismissActivation())
                .Show();
        }

        private void ToastNotificationOnActivatedHanlder(ToastNotificationActivatedEventArgsCompat toastArgs)
        {
            try
            {
                var args = ToastArguments.Parse(toastArgs.Argument);

                Action<String>? action = args[ActionArgumentName] switch
                {
                    ViewDestinationFolderDelegateName => ViewDestinationFolderAction,
                    OpenLogFileDelegateName => OpenLogFileAction,
                    _ => null,
                };

                if (action != null)
                {
                    action(args[ArgArgumentName]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        private void ViewDestinationFolderAction(String arg)
        {
            if (String.IsNullOrWhiteSpace(arg) || !Directory.Exists(arg))
            {
                return;
            }
            var startInfo = new ProcessStartInfo
            {
                Arguments = arg,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }

        private void OpenLogFileAction(String _)
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments = _logDirectoryInfo.FullName,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }
    }
}
