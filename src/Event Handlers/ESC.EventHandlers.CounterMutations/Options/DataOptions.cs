namespace ESC.EventHandlers.CounterMutations.Options
{
    /// <summary>
    /// Contains application settings for connecting to a data store
    /// </summary>
    public class DataOptions
    {
        /// <summary>
        /// Event Store connection string
        /// </summary>
        public string EventStoreConnectionString { get; set; }

        /// <summary>
        /// MongoDB connection string
        /// </summary>
        public string MongoConnectionString { get; set; }
    }
}
