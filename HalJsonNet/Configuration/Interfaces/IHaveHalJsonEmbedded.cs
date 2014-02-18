using System.Collections.Generic;

namespace HalJsonNet.Configuration.Interfaces
{
    public interface IHaveHalJsonEmbedded
    {
        IDictionary<string, Embedded> GetEmbedded();
    }
}
