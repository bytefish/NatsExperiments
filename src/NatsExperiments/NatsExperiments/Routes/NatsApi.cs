// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NATS.Client.JetStream;
using NatsExperiments.Model;

namespace NatsExperiments.Routes
{
    public static class NatsApi
    {
        public static RouteGroupBuilder MapNatsApi(this RouteGroupBuilder group)
        {
            group.MapPost("/publish/", async (AppEvent @event, INatsJSContext jetStream) =>
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

            return group;
        }
    }
}