// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using NatsExperiments.Hosted;
using NatsExperiments.Infrastructure;
using NatsExperiments.Infrastructure.Nats;
using NatsExperiments.Model;
using NatsExperiments.Routes;

var builder = WebApplication.CreateBuilder(args);

// Nats Client
builder.AddNatsClient("nats", configureOptions: opts =>
{  
    var jsonRegistry = new NatsJsonContextSerializerRegistry(AppJsonContext.Default);
    
    return opts with 
    { 
        AuthOpts = new NatsAuthOpts
        { 
            Username = "admin",
            Password = "5!F25GbKwU3P"
        },
        SerializerRegistry = jsonRegistry 
    };
});

// Nats JetStream
builder.AddNatsJetStream();

// Nats Message Handlers
builder.Services.AddSingleton<LoggingNatsMessageHandler<AppEvent>>();

// Nats Background Service 
builder.Services.AddHostedService((serviceProvider) => 
{
    // Listen to the AppEvents Stream
    var appEventsStreamConfig = new StreamConfig(Constants.AppEventsStreamName, ["events.>"])
    {
        Description = "AppEvents Stream",
    };

    return new NatsBackendService<AppEvent>(
        loggerFactory: serviceProvider.GetRequiredService<ILoggerFactory>(),
        natsJsContext: serviceProvider.GetRequiredService<INatsJSContext>(),
        natsJsMessageHandler: serviceProvider.GetRequiredService<LoggingNatsMessageHandler<AppEvent>>(),
        streamConfig: appEventsStreamConfig,
        consumerOptions: NatsJSOrderedConsumerOpts.Default);
});

// Health Checks 
builder.Services.AddHealthChecks();

var app = builder.Build();

// Health Endpoint
app.MapHealthChecks("/healthz");

// Nats Endpoints
app
    .MapGroup("/nats")
    .MapNatsApi();

app.Run();