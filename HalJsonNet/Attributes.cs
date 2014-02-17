using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalJsonNet
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