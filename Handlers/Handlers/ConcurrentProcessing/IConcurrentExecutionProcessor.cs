using Shared.Models.Work;

namespace Handlers.ConcurrentProcessing
{
    internal interface IConcurrentExecutionProcessor
    {
        Task<Boolean> ProcessInParallelAsync(IEnumerable<UnitOfWork> unitsOfWork, Func<UnitOfWork, Task<Boolean>> processingFunc);
        Task<Boolean> ProcessInParallelAsync(IEnumerable<UnitOfWork> unitsOfWork, Func<UnitOfWork, Task<Boolean>> processingFunc, Int32 threads);
    }
}
