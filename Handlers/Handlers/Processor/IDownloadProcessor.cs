using Handlers.Models;
using Shared.Models.Work;

namespace Handlers.Processor
{
    internal interface IDownloadProcessor
    {
        StrategyEnum DownloadStrategy { get; }
        Task<Boolean> DownloadAsync(UnitOfWork unitOfWork, String path, HttpClient client, CancellationToken cancellationToken);
    }
}
