using System.Collections.Generic;
using ModManagement.Models;
using SemanticVersioning;

namespace ModManagement
{
    public class ModProperties
    {
        public ModManifest Manifest { get; private set; }
        public string Directory { get; private set; }
        
        internal ModProperties(string directory, ModManifest manifest)
        {
            Directory = directory;
            Manifest = manifest;
        }
    }
}