using DownloaderCli.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Models.Work;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloaderCli.Resume
{
    internal class ResumeHandler: IResumeHandler
    {
        private readonly DownloaderCliConfig _options;
        private readonly ILogger<ResumeHandler> _logger;

        public ResumeHandler(IOptions<DownloaderCliConfig> options, ILogger<ResumeHandler> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public WorkSet HandlerResume(WorkSet workSet, String path)
        {
            if (!_options.Resume)
            {
                _logger.LogInformation("Resume option is disabled. Ignoring...");
                return workSet;
            }
            _logger.LogInformation("Resume option is enabled. Filtering downloaded items...");
            var count = workSet.Count;
            var remainingDownloadsCount = FilterDownloaded(workSet, path);
            _logger.LogInformation($"[{count - remainingDownloadsCount}] item{(count - remainingDownloadsCount != 1 ? "s were" : " was")} filtered out. [{remainingDownloadsCount}] item{(remainingDownloadsCount != 1 ? "s" : "")} left.");
            return workSet;
        }

        private Int32 FilterDownloaded(WorkSet workSet, String path)
        {
            Int32 count = workSet.Count;
            var downloadPath = String.IsNullOrWhiteSpace(workSet.FolderName) ? path : Path.Combine(path, workSet.FolderName);
            if (!Directory.Exists(downloadPath))
            {
                return count;
            }
            var existingFiles = new DirectoryInfo(downloadPath).GetFiles().Select(fileInfo => fileInfo.Name).ToHashSet();
            foreach (UnitOfWork unitOfWork in workSet)
            {
                if (existingFiles.Contains(unitOfWork.Filename))
                {
                    unitOfWork.IsDownloaded = true;
                    count--;
                }
            }
            if(count != workSet.Count)
            {
                workSet.RecheckState();
            }
            return count;
        }
    }
}
