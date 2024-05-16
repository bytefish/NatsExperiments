// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using NatsExperiments.Infrastructure;
using NatsExperiments.Model;

namespace NatsExperiments.Hosted
{
    public class AppEventsBackendService : BackgroundService
    {
        private readonly ILogger<AppEventsBackendService> _logger;

        private readonly StreamConfig _streamConfig;
        private readonly INatsJSContext _natsJetStreamContext;
        private readonly NatsJSOrderedConsumerOpts _consumerOptions;

        public AppEventsBackendService(ILogger<AppEventsBackendService> logger, INatsJSContext natsJsContext, StreamConfig streamConfig, NatsJSOrderedConsumerOpts? consumerOptions = null)
        {
            _logger = logger;
            _streamConfig = streamConfig;
            _natsJetStreamContext = natsJsContext;
            _consumerOptions = consumerOptions ?? NatsJSOrderedConsumerOpts.Default;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Create the Stream, if it does not exist:
            await _natsJetStreamContext.CreateStreamAsync(_streamConfig, cancellationToken);
            
            // Create the Ordered Consumer:
            var consumer = await _natsJetStreamContext
                .CreateOrderedConsumerAsync(stream: Constants.AppEventsStreamName, _consumerOptions, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            // Consume Messages and Log them
            await foreach (var msg in consumer
                .ConsumeAsync<AppEvent>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))
            {
                _logger.LogInformation("received msg on {Subject} with data {Data}", msg.Subject, msg.Data);

                await msg.AckAsync();
            }
        }
    }
}