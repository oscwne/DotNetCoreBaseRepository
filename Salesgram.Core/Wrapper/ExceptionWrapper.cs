using System;
namespace Salesgram.Core.Wrapper
{
    public class ExceptionWrapper
    {
        public Uri RequestUri { get; set; }
        public DateTime DateTime { get; set; }
        public Exception Exception { get; set; }
    }
}
