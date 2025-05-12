using System.Collections.Generic;
using ModManagement.Models;
using SemanticVersioning;

namespace ModManagement
{
    public class ModProperties
    {
        public ModDescriptor Descriptor { get; private set; }
        public string Directory { get; private set; }
        
        internal ModProperties(string directory, ModDescriptor descriptor)
        {
            Directory = directory;
            Descriptor = descriptor;
        }
    }
}