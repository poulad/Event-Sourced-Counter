namespace ESC.Events
{
    public static class StreamNames
    {
        public const string CounterStreamPrefix = "counter-";

        public const string WebRequestStreamName = "WebRequest";

        public static string GetStreamNameFromCounterId(string counterId) => $"{CounterStreamPrefix}{counterId}";

        public static string GetCounterName(string streamName)
        {
            string counterName;
            if (streamName?.StartsWith(CounterStreamPrefix) == true)
            {
                counterName = streamName.Substring(CounterStreamPrefix.Length);
            }
            else
            {
                counterName = null;
            }

            return counterName;
        }
    }
}