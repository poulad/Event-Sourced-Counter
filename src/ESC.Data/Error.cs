using System.Collections.Generic;

namespace ESC.Data
{
    public class Error
    {
        public string Code { get; }

        public string Message { get; }

        public IDictionary<string, object> Items { get; }

        public Error(
            string code,
            string message = default,
            IDictionary<string, object> items = default
        )
        {
            Code = code;
            Message = message;
            Items = items;
        }
    }
}
