using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainProcessors.Abstractions.Exceptions
{
    /// <summary>
    /// Base type for processing exceptions.
    /// </summary>
    public abstract class AbstractProcessingException: Exception
    {
        /// <summary>
        /// When overwritten in derived class gets retriablility as <see cref="Boolean"/>.
        /// </summary>
        public abstract Boolean IsRetriable { get; }

        ///<inheritdoc/>
        public AbstractProcessingException(String message) : base(message) { }

        ///<inheritdoc/>
        public AbstractProcessingException(String message, Exception? innerException) : base(message, innerException) { }
    }
}
