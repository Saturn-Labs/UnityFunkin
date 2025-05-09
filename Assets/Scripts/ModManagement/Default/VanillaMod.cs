using System;
using ModManagement.Abstractions;
using ModManagement.Attributes;
using UnityEngine.Device;

namespace ModManagement.Default
{
    [Serializable]
    [ModDefinition(
        Name = "Vanilla Mod",
        Description = "This is the default mod.",
        Identifier = "mod@ryd3v@vanilla",
        UUID = "22ca54ad-446f-4592-afc8-e21456eb5055"
    )]
    public class VanillaMod : AbstractMod
    {
        public VanillaMod()
        {
            GetType().GetProperty("Directory")?.GetSetMethod(true).Invoke(this, new object[]
            {
                Application.dataPath
            });
        }
    }
}