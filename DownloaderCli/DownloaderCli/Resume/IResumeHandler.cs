using Shared.Models.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloaderCli.Resume
{
    internal interface IResumeHandler
    {
        WorkSet HandlerResume(WorkSet workSet, String path);
    }
}
