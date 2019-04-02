using System;
using System.Runtime.Serialization;

namespace SafeMobileBrowser.WebFetchImplementation
{
    public class WebFetchException : Exception
    {
        public int ErrorCode { get; set; }

        public WebFetchException()
        {
        }

        public WebFetchException(string message) : base(message)
        {
        }

        public WebFetchException(string message, Exception inner) : base(message, inner)
        {
        }

        public WebFetchException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public WebFetchException(int ErrorCode, string message) : base(message)
        {
            this.ErrorCode = ErrorCode;
        }
    }
}
