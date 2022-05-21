using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainProcessors.Abstractions.Exceptions
{
    /// <summary>
    /// The exception that is thrown when AngleSharp.Dom.IParentNode processing fails. This exception is non-retriable.
    /// </summary>
    public class IParentNodeProcessingException: AbstractProcessingException
    {
        ///<inheritdoc/>
        public override Boolean IsRetriable { get; } = false;

        /// <summary>
        /// Initializes a new instance of the DomainProcessors.Abstractions.Exceptions.IParentNodeProcessingException with a specified message.
        /// </summary>
        /// <param name="message">String that describes the error. The content of message is intended to be understood by humans.</param>
        public IParentNodeProcessingException(String message): base(message) { }
    }
}
