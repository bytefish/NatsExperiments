﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NatsExperiments.Infrastructure.Health
{
    internal static class HealthChecksExtensions
    {
        /// <summary>
        /// Adds a HealthCheckRegistration if one hasn't already been added to the builder.
        /// </summary>
        public static void TryAddHealthCheck(this IHostApplicationBuilder builder, HealthCheckRegistration healthCheckRegistration)
        {
            builder.TryAddHealthCheck(healthCheckRegistration.Name, hcBuilder => hcBuilder.Add(healthCheckRegistration));
        }

        /// <summary>
        /// Invokes the <paramref name="addHealthCheck"/> action if the given <paramref name="name"/> hasn't already been added to the builder.
        /// </summary>
        public static void TryAddHealthCheck(this IHostApplicationBuilder builder, string name, Action<IHealthChecksBuilder> addHealthCheck)
        {
            var healthCheckKey = $"HealthChecks.{name}";
            if (!builder.Properties.ContainsKey(healthCheckKey))
            {
                builder.Properties[healthCheckKey] = true;
                addHealthCheck(builder.Services.AddHealthChecks());
            }
        }
    }
}