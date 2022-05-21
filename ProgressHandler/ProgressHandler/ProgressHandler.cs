using Microsoft.Extensions.Options;
using ProgressHandler.Abstractions;
using ProgressHandler.Configuration;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ProgressHandler
{
    public class ProgressHandler : IProgressHandler
    {
        private const Int32 MaxProgress = 100;
        private Single _progress;

        private UInt32 _iterationsCount;
        private UInt32 _totalIterationsCount;
        private Single _progressStep;
        private String _progressStepText;
        private String _progressValueText;
        private Boolean _progressStarted;

        private readonly IPopupNotificationProvider _notificationProvider;
        private readonly ProgressHandlerConfig _options;

        public ProgressHandler(IOptions<ProgressHandlerConfig> options, IPopupNotificationProvider notificationProvider)
        {
            _notificationProvider = notificationProvider;
            _options = options.Value;
        }

        #region Implementation of IProgressHandler

        /// <inheritdoc />
        public String ProgressValueText
        {
            get => $"{_progressValueText} %";
            set
            {
                _progressValueText = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public String ProgressStepText
        {
            get => _progressStepText;
            set
            {
                _progressStepText = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public void Start(String step, UInt32 totalIterationsCount)
        {
            UpdateStep(totalIterationsCount);
            ProgressStepText = step;
            ProgressValueText = "0";
            _progress = 0;
            _iterationsCount = 0;
            _totalIterationsCount = totalIterationsCount;
            _progressStarted = true;
            if (_options.ShowPopupNotifications)
            {
                _notificationProvider.CreateProgressToast(_progressStepText, _totalIterationsCount);
            }
        }

        /// <inheritdoc />
        public void Step(String name)
        {
            Debug.Assert(_progressStarted, "Cannot change step name of untracked progress. Did you forget to call Start?");
            ProgressStepText = name;
            if (_options.ShowPopupNotifications)
            {
                _notificationProvider.UpdateProgressStep(ProgressStepText);
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            Debug.Assert(_progressStarted, "Cannot stop untracked progress. Did you forget to call Start?");
            Update(MaxProgress);
            _progressStarted = false;
        }

        /// <inheritdoc />
        public void Update()
        {
            Debug.Assert(_progressStarted, "Cannot update untracked progress. Did you forget to call Start?");
            _progress += _progressStep;
            if (_iterationsCount < _totalIterationsCount)
            {
                _iterationsCount++;
            }
            if (_progress > MaxProgress)
            {
                _progress = MaxProgress;
            }

            ProgressValueText = ((Int32)_progress).ToString();
            if (_options.ShowPopupNotifications)
            {
                _notificationProvider.UpdateProgress(_progress, _iterationsCount);
            }
        }

        /// <inheritdoc />
        public void Update(UInt32 progress)
        {
            Debug.Assert(_progressStarted, "Cannot update untracked progress. Did you forget to call Start?");
            _iterationsCount = progress > MaxProgress ? _totalIterationsCount : _iterationsCount + (UInt32)((progress - _progress) / _progressStep);
            _progress = progress > MaxProgress ? MaxProgress : progress;
            ProgressValueText = progress.ToString();
            if (_options.ShowPopupNotifications)
            {
                _notificationProvider.UpdateProgress(_progress, _iterationsCount);
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void UpdateStep(UInt32 iterationsCount)
        {
            _progressStep = (Single)MaxProgress / iterationsCount;
        }
    }
}
