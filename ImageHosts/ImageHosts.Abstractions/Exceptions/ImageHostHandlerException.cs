using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageHosts.Abstractions.Exceptions
{
    /// <summary>
    /// The exception that is thrown when image host internal processing returns no result.
    /// </summary>
    public class ImageHostHandlerException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the ImageHosts.Abstractions.Exceptions.ImageHostHandlerException with a specified message.
        /// </summary>
        /// <param name="message">String that describes the error. The content of message is intended to be understood by humans.</param>
        public ImageHostHandlerException(String message): base(message) { }

        /// <summary>
        /// Initializes a new instance of the ImageHosts.Abstractions.Exceptions.ImageHostHandlerException with a specified message.
        /// </summary>
        /// <param name="message">String that describes the error. The content of message is intended to be understood by humans.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ImageHostHandlerException(String message, Exception innerException): base(message, innerException) { }
    }
}
