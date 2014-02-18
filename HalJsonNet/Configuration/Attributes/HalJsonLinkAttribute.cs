using System;

namespace HalJsonNet.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class HalJsonLinkAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Link { get; private set; }
        public bool Templated { get; private set; }
        public bool HideProperty { get; private set; }

        public HalJsonLinkAttribute(string name, string link, bool templated = false, bool hideProperty = false)
        {
            Name = name;
            Link = link;
            Templated = templated;
            HideProperty = hideProperty;
        }
    }
}