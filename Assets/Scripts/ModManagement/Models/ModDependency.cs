using Newtonsoft.Json;
using SemanticVersioning;
using Utils.Converters;

namespace ModManagement.Models
{
    /// <summary>
    /// Represents a dependency of a mod.
    /// </summary>
    public class ModDependency
    {
        /// <summary>
        /// The identifier of the dependency mod.
        /// </summary>
        [JsonProperty("identifier")]
        public string Identifier { get; private set; }

        /// <summary>
        /// The version range of the dependency mod.
        /// </summary>
        [JsonProperty("version")]
        [JsonConverter(typeof(VersionRangeConverter))]
        public Range Version { get; private set; } = null!;
    }
}