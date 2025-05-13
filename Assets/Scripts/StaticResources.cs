using UnityEngine;

public static class StaticResources
{
    public static readonly TextAsset ModManifestDescriptor = Resources.Load<TextAsset>("Schemas/mod_manifest_schema");
    public static readonly Material SparrowMaterial = Resources.Load<Material>("Materials/Sparrow-Default");
    
    static StaticResources()
    {
        
    }
}