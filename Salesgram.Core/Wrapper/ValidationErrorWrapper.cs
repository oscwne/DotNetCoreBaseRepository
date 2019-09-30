using System;
using Newtonsoft.Json;

namespace Salesgram.Core.Wrapper
{
    public class ValidationErrorWrapper
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }

        public ValidationErrorWrapper(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }
}
