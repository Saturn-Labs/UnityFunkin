using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using UnityEngine;
using Utils.Converters;
using Version = SemanticVersioning.Version;

namespace ModManagement.Models
{
    /// <summary>
    /// Represents a mod manifest, that stores all metadata about the mod.
    /// </summary>
    public class ModManifest
    {
        public static TextAsset Schema => StaticResources.ModManifestDescriptor;
        
        /// <summary>
        /// The format version of the mod manifest.
        /// </summary>
        [JsonProperty("format_version")]
        public int FormatVersion { get; private set; } = 0;

        /// <summary>
        /// The name of the mod.
        /// </summary>
        /// <since>"format_version": 0</since>
        [JsonProperty("name")]
        public string Name { get; private set; } = "Unknown Mod";
        
        /// <summary>
        /// The description of the mod.
        /// </summary>
        /// <since>"format_version": 0</since>
        [JsonProperty("description")]
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// The identifier of the mod.
        /// </summary>
        /// <since>"format_version": 0</since>
        [JsonProperty("identifier")]
        public string Identifier { get; private set; } = "mod@unknown@unknownmod";
        
        /// <summary>
        /// The universally unique identifier (v4) of the mod.
        /// </summary>
        /// <since>"format_version": 0</since>
        [JsonProperty("uuid")]
        public string UniqueId { get; private set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The version of the mod.
        /// </summary>
        /// <since>"format_version": 0</since>
        [JsonProperty("version")]
        [JsonConverter(typeof(VersionConverter))]
        public Version Version { get; private set; } = Version.Parse("0.0.1");
        
        /// <summary>
        /// The dependencies of the mod.
        /// </summary>
        /// <since>"format_version": 0</since>
        [JsonProperty("dependencies")]
        public ModDependency[] Dependencies { get; private set; } = Array.Empty<ModDependency>();

        /// <summary>
        /// Deserializes a JSON string into a <see cref="ModManifest"/> object.
        /// </summary>
        public static ModManifest FromJson(string json)
        {
            var root = JObject.Parse(json);
            if (root is null)
                throw new JsonException("Failed to parse JSON string.");

            var schema = JSchema.Parse(Schema.text);
            var result = root.IsValid(schema, out IList<string> messages);
            if (!result)
            {
                throw new JSchemaException($"ModManifest schema validation failed: {string.Join(", ", messages)}");
            }

            var obj = root.ToObject<ModManifest>();
            if (obj is null)
                throw new JsonException("Failed to deserialize JSON string into ModManifest object.");
            return obj;
        }

        /// <summary>
        /// Generates a string representation of the <see cref="ModManifest"/> object.
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}