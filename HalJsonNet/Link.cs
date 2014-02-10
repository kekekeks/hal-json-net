using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalJsonNet
{
	public sealed class Link
	{
		public bool Templated { get; private set; }
		private readonly Func<object, string> _linkGetter;
		
		public Link(Func<object, string> linkGetter, bool templated = false)
		{
			Templated = templated;
			_linkGetter = linkGetter;
		}

		public string GetHref(object target)
		{
			return _linkGetter(target);
		}
	}
}
