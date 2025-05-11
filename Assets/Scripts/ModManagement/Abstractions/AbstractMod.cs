using System;
using System.Collections.Generic;
using System.Reflection;
using ModManagement.Attributes;
using ModManagement.Utils;
using Range = SemanticVersioning.Range;
using Version = SemanticVersioning.Version;

namespace ModManagement.Abstractions
{
    [Serializable]
    public abstract class AbstractMod
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Identifier { get; private set; }
        public string UUID { get; private set; }
        public Version Version { get; private set; }
        public string Directory { get; private set; } = null!;
        public readonly List<ModDependency> Dependencies = new();

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
            Version = Version.Parse(attr.Version);
        }
        
        public virtual void OnLoad() { }
        public virtual void OnUnload() { }

        public void RegisterDependency(ModDependency dependency)
        {
            Dependencies.Add(dependency);
        }
        
        public void RegisterDependencies(IEnumerable<ModDependency> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                RegisterDependency(dependency);
            }
        }

        public void RegisterDependency<TMod>(Range range) where TMod : AbstractMod
        {
            if (typeof(TMod).GetCustomAttribute<ModDefinition>() is not { } attr)
                throw new ArgumentException("Mod definition attribute is missing when trying to add it as a dependency.");
            RegisterDependency(new ModDependency(attr.Identifier, instance => instance is TMod mod && range.IsSatisfied(mod.Version)));
        }
    }
}