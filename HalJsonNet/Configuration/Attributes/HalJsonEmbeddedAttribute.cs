using System;

namespace HalJsonNet.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HalJsonEmbeddedAttribute : Attribute
    {
        public string Name { get; private set; }
        public bool HideProperty { get; private set; }

        public HalJsonEmbeddedAttribute(string name, bool hideProperty = true)
        {
            Name = name;
            HideProperty = hideProperty;
        }
    }
}