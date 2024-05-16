namespace NatsExperiments.Infrastructure.Nats
{
    /// <summary>
    /// Provides the client configuration settings for connecting to a NATS cluster.
    /// </summary>
    public sealed class NatsClientSettings
    {
        /// <summary>
        /// Gets or sets the connection string of the NATS cluster to connect to.
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the NATS health check is disabled or not.
        /// </summary>
        /// <value>
        /// The default value is <see langword="false"/>.
        /// </value>
        public bool DisableHealthChecks { get; set; }
    }
}