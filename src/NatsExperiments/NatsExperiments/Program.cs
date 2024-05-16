// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using NatsExperiments.Hosted;
using NatsExperiments.Infrastructure;
using NatsExperiments.Infrastructure.Nats;
using NatsExperiments.Model;

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

// Nats Background Service 
builder.Services.AddSingleton(new StreamConfig(Constants.AppEventsStreamName, ["events.>"])
{
    Description = "AppEvents Stream",
});

builder.Services.AddHostedService<AppEventsBackendService>();

// Health Checks 
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapPost("/publish/", async (AppEvent @event, INatsJSContext jetStream) =>
{
    try
    {
        var ack = await jetStream.PublishAsync(@event.Subject, @event);

        ack.EnsureSuccess();
    }
    catch (NatsJSPublishNoResponseException)
    {
        return Results.Problem("Make sure the stream is created before publishing.");
    }

    return Results.Created();
});

app.Run();