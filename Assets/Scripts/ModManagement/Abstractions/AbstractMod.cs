using System;
using System.Reflection;
using NuGet.Versioning;

namespace ModManagement.Abstractions
{
    [Serializable]
    public abstract class AbstractMod
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Identifier { get; private set; }
        public string UUID { get; private set; }
        public NuGetVersion Version { get; private set; }
        public string Directory { get; private set; } = null!;

        protected AbstractMod()
        {
            var type = GetType();
            if (type.GetCustomAttribute<Attributes.ModDefinition>() is not { } attr)
                throw new ArgumentException("Mod definition attribute is missing.");
            if (string.IsNullOrEmpty(attr.Name))
                throw new ArgumentException("Mod name is missing.");
            if (string.IsNullOrEmpty(attr.Identifier))
                throw new ArgumentException("Mod identifier is missing.");
            if (string.IsNullOrEmpty(attr.UUID))
                throw new ArgumentException("Mod UUID is missing.");
            if (string.IsNullOrEmpty(attr.Version))
                throw new ArgumentException("Mod version is missing.");
            Name = attr.Name;
            Description = attr.Description;
            Identifier = attr.Identifier;
            UUID = attr.UUID;
            Version = NuGetVersion.Parse(attr.Version);
        }
        
        public virtual void OnLoad() { }
        public virtual void OnUnload() { }
    }
}