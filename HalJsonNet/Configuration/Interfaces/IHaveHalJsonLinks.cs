using System.Collections.Generic;

namespace HalJsonNet.Configuration.Interfaces
{
    public interface IHaveHalJsonLinks
    {
        IDictionary<string, Link> GetLinks();
    }
}
