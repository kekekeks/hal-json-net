using System;

namespace HalJsonNet.Configuration
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

	    public Link(string link, bool templated = false) : this(x => link, templated)
	    {

	    }

	    public static implicit operator Link(string link)
	    {
	        return new Link(link);
	    }

	    public string GetHref(object target)
		{
			return _linkGetter(target);
		}
	}
}
