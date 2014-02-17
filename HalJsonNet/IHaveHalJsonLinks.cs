using System.Collections.Generic;

namespace HalJsonNet
{
    public interface IHaveHalJsonLinks
    {
        IDictionary<string, Link> GetLinks();
    }
}
