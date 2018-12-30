namespace ESC.Events
{
    public static class StreamNames
    {
        public const string CountersStreamPrefix = "counters-";

        public const string CounterMutationsStreamName = "counterMutations";

        public static string GetStreamNameFromCounterId(string counterId) => $"{CountersStreamPrefix}{counterId}";

        public static string GetStreamNameForEventType(string eventType) => $"$et-{eventType}";
    }
}
