// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NATS.Client.JetStream;
using NatsExperiments.Infrastructure.Logging;

namespace NatsExperiments.Infrastructure.Nats
{
    public interface INatsJSMessageHandler<TEvent>
    {
        /// <summary>
        /// Handles a NATS JetStream Message.
        /// </summary>
        /// <param name="message">Message to process</param>
        /// <returns>Awaitable Task</returns>
        ValueTask HandleAsync(NatsJSMsg<TEvent> message, CancellationToken cancellationToken);
    }

    public class LoggingNatsMessageHandler<TEvent> : INatsJSMessageHandler<TEvent>
    {
        private readonly ILogger<LoggingNatsMessageHandler<TEvent>> _logger;

        public LoggingNatsMessageHandler(ILogger<LoggingNatsMessageHandler<TEvent>> logger)
        {
            _logger = logger;
        }

        public async ValueTask HandleAsync(NatsJSMsg<TEvent> message, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            _logger.LogInformation("received msg on {Subject} with data {Data}", message.Subject, message.Data);

            await message.AckAsync(cancellationToken: cancellationToken);
        }
    }
}
