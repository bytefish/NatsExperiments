// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using NatsExperiments.Infrastructure.Logging;
using NatsExperiments.Infrastructure.Nats;

namespace NatsExperiments.Hosted
{
    public class NatsMessageConsumerService<TEvent> : BackgroundService
    {
        private readonly ILogger<NatsMessageConsumerService<TEvent>> _logger;

        private readonly StreamConfig _streamConfig;
        private readonly INatsJSContext _natsJetStreamContext;
        private readonly INatsJSMessageHandler<TEvent> _natsJsMessageHandler;
        private readonly NatsJSOrderedConsumerOpts _consumerOptions;

        public NatsMessageConsumerService(ILoggerFactory loggerFactory, 
            INatsJSContext natsJsContext, 
            INatsJSMessageHandler<TEvent> natsJsMessageHandler,
            StreamConfig streamConfig, 
            NatsJSOrderedConsumerOpts? consumerOptions = null)
        {
            _logger = loggerFactory.CreateLogger<NatsMessageConsumerService<TEvent>>();
            _natsJetStreamContext = natsJsContext;
            _natsJsMessageHandler = natsJsMessageHandler;
            _streamConfig = streamConfig;
            _consumerOptions = consumerOptions ?? NatsJSOrderedConsumerOpts.Default;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Create the Stream, if it does not exist:
            await _natsJetStreamContext.CreateStreamAsync(_streamConfig, cancellationToken);

            // Create the Ordered Consumer:
            var consumer = await _natsJetStreamContext
                .CreateOrderedConsumerAsync(stream: _streamConfig.Name!, _consumerOptions, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            // Consume Messages and Log them
            await foreach (var message in consumer
                .ConsumeAsync<TEvent>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))
            {
                await _natsJsMessageHandler
                    .HandleAsync(message, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}