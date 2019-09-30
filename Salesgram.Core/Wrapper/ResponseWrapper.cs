namespace Salesgram.Core.Wrapper
{
    public class ResponseWrapper<T>
    {
        public string Message { get; set; }
        public T Result { get; set; }
        public object Errors { get; set; }

        public ResponseWrapper()
        { }

        public ResponseWrapper(string message, T result, object errors)
        {
            Message = message;
            Result = result;
            Errors = errors;
        }
    }
}
