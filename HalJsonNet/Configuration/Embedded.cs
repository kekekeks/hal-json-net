using System;

namespace HalJsonNet.Configuration
{
	public class Embedded
	{
		
		public Embedded(Func<object, object> getter)
		{
			Getter = getter;
		}

	    public Embedded(object data) : this(_ => data)
	    {

	    }

	    public Func<object, object> Getter { get; private set; }
	}
}
