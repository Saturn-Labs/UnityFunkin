using System;

namespace TransitionManagement.Attributes
{
    public class TransitionTypeDeclaration : Attribute
    {
        public string Name { get; set; } = string.Empty;

        public TransitionTypeDeclaration()
        {
        }
        
        public TransitionTypeDeclaration(string name) : this()
        {
            Name = name;
        }
    }
}