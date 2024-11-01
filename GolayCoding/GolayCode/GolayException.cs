using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GolayCode
{
    /// <summary>
    /// Represents errors that occur in Golay code processing.
    /// </summary>
    public class GolayException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GolayException"/> class with a specific message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public GolayException(string message) : base(message)
        {

        }
    }
}
