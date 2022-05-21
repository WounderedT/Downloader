using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainProcessors.Abstractions.Exceptions
{
    /// <summary>
    /// The exception that is thrown when internal domain processing fails. This exception is non-retriable.
    /// </summary>
    public class InternalUrlProcessingException: AbstractProcessingException
    {
        ///<inheritdoc/>
        public override Boolean IsRetriable { get; } = false;

        /// <summary>
        /// Initializes a new instance of the DomainProcessors.Abstractions.Exceptions.InternalUrlProcessingException with a specified message and 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InternalUrlProcessingException(Exception innerException) : base($"URL processing failed", innerException) { }

        /// <summary>
        /// Initializes a new instance of the DomainProcessors.Abstractions.Exceptions.InternalUrlProcessingException with a specified message and 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="url">System.String URL which caused the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InternalUrlProcessingException(String url, Exception innerException): base($"Processing of [{url}] failed", innerException) { }
    }
}
