using System;
using ModManagement.Abstractions;

namespace ModManagement.Utils
{
    public class ModDependency
    {
        public string Identifier { get; set; }
        public Predicate<AbstractMod> Selector { get; set; }
        
        public ModDependency() : this(string.Empty, _ => true) { }
        public ModDependency(string identifier, Predicate<AbstractMod> selector)
        {
            Identifier = identifier;
            Selector = selector;
        }
    }
}