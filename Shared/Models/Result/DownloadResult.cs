using Shared.Models.Work;

namespace Shared.Models.Result
{
    public class DownloadResult
    {
        private WorkRequest? _originalWorkRequest;
        public Boolean IsSuccess { get; set; } = true;
        public String DownloadPath { get; }
        public List<String> Urls { get; } = new List<String>();
        public WorkRequest? OriginalWorkRequest
        {
            get { return _originalWorkRequest; }
            set
            {
                _originalWorkRequest = value;
                Urls.Clear();
                if(value != null)
                {
                    Urls.AddRange(value.Urls);
                }
            }
        }

        public Int32 TotalCount { get; private set; }
        public Int32 SuccessCount { get; private set; }
        public Int32 FailureCount { get; private set; }
        public Exception? Exception { get; set; }
        public List<FailedWorkSet> FailedUnitsOfWork { get; } = new List<FailedWorkSet>();

        public DownloadResult(WorkRequest workRequest, String downloadPath): this(downloadPath)
        {
            Urls.AddRange(workRequest.Urls);
            OriginalWorkRequest = workRequest;
        }

        public DownloadResult(String downloadPath)
        {
            DownloadPath = downloadPath;
        }

        public void AddResult(WorkSet workset)
        {
            if (workset == null)
            {
                throw new ArgumentNullException(nameof(workset));
            }

            var failedUnits = workset.Where(u => !u.IsDownloaded);
            workset.IsSuccess = !failedUnits.Any();
            IsSuccess &= workset.IsSuccess;
            if (workset.IsSuccess)
            {
                SuccessCount++;
            }
            else
            {
                FailureCount++;
                var failedWorkset = new FailedWorkSet(workset.Domain, String.IsNullOrEmpty(workset.FolderName) ? DownloadPath : Path.Combine(DownloadPath, workset.FolderName));
                failedWorkset.FailedUnitsOfWork.AddRange(failedUnits);
                FailedUnitsOfWork.Add(failedWorkset);
            }
            TotalCount++;
        }

        public void AddException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            IsSuccess = false;
            Exception = exception;
        }
    }
}
