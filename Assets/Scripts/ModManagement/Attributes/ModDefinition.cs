using System;
using ModManagement.Utils;

namespace ModManagement.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModDefinition : Attribute
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public string UUID { get; set; } = string.Empty;
        public string Version { get; set; } = "0.0.0";
    }
}