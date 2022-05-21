using Shared.Models.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Result
{
    public class FailedWorkSet
    {
        public String Domain { get; }
        public String DownloadPath { get; }
        public List<UnitOfWork> FailedUnitsOfWork { get; } = new List<UnitOfWork>();

        public FailedWorkSet(String domain, String downloadPath)
        {
            if (String.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException(nameof(domain));
            }
            if (String.IsNullOrEmpty(downloadPath))
            {
                throw new ArgumentNullException(nameof(downloadPath));
            }
            Domain = domain;
            DownloadPath = downloadPath;
        }
    }
}
