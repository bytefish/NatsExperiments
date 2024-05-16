// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace NatsExperiments.Model
{
    public record AppEvent
    {
        public required string Subject { get; set; }
        
        public required string Name { get; set; }
        
        public required string Description { get; set; }

        public required string Priority { get; set; }
    }

    [JsonSerializable(typeof(AppEvent))]
    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    public partial class AppJsonContext : JsonSerializerContext
    {
    }
}