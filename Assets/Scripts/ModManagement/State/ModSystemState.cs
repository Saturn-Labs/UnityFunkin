using System;

namespace ModManagement.State
{
    [Serializable]
    public enum ModSystemState
    {
        Idling,
        LoadingAssembly,
        InstantiatingMod
    }
}