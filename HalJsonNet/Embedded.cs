﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalJsonNet
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
