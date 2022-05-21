using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainProcessors.Abstractions.Exceptions
{
    /// <summary>
    /// The exception that is thrown when URL processing failure is caused by external factors (network, service reliablility, etc.). This exception is retriable.
    /// </summary>
    public class UrlProcessingException: AbstractProcessingException
    {
        ///<inheritdoc/>
        public override Boolean IsRetriable { get; } = true;

        /// <summary>
        /// Initializes a new instance of the DomainProcessors.Abstractions.Exceptions.UrlProcessingException with a specified URL and 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="url">System.String URL which caused the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public UrlProcessingException(String url, Exception innerException) : base($"Processing of [{url}] failed", innerException) { }
    }
}
