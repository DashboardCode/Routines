using System;

namespace Vse.Routines.IncludableTypes
{
    public class ExtendsAttribute : Attribute
    {
        readonly Type definitionType;
        readonly string definitionInculde;
        public ExtendsAttribute(Type definitionType, string definitionInculde)
        {
            this.definitionType = definitionType;
            this.definitionInculde = definitionInculde;
        }
    }
}
