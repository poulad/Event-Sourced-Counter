namespace ESC.Events
{
    public static class StreamNames
    {
        public const string CounterStreamPrefix = "counter:";

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