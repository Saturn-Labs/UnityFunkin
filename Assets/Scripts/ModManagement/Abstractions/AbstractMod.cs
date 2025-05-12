using System;

namespace ModManagement.Abstractions
{
    [Serializable]
    public abstract class AbstractMod
    {
        public ModProperties Properties { get; private set; } = null!;

        protected AbstractMod()
        {
        }
        
        public virtual void OnLoad() { }
        public virtual void OnUnload() { }
    }
}