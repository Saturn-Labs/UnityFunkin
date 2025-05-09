using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FileSystem
{
    public static class PathUtils
    {
        private static readonly Dictionary<string, Func<string>> _PathAliases = new()
        {
            { "app:", () => Application.dataPath },
            { "appdata:", () => Application.persistentDataPath },
            { "mods:", () => Path.Combine(Application.dataPath, "..", "mods") },
            { "cache:", () => Application.temporaryCachePath },
            { "user:", () => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) },
            { "docs:", () => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) }
        };
        
        public static string Process(string path)
        {
            foreach (var (alias, full) in _PathAliases)
            {
                if (path.StartsWith(alias))
                {
                    return Path.Combine(full(), path.Replace(alias, ""));
                }
            }
            return path;
        }
    }
}