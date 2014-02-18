using System;
using System.Collections.Immutable;

namespace HalJsonNet.Utility
{
	class ThreadSafeCache<TKey, TValue>
	{
		private ImmutableDictionary<TKey, TValue> _dic = ImmutableDictionary<TKey, TValue>.Empty;
		private readonly object _syncRoot = new object();
		public TValue GetOrAdd(TKey key, Func<TKey, TValue> getter)
		{
			TValue rv;
			if (_dic.TryGetValue(key, out rv))
				return rv;
			lock (_syncRoot)
			{
				if (_dic.TryGetValue (key, out rv))
					return rv;
				_dic = _dic.Add(key, rv = getter(key));
				return rv;
			}
		}

		public bool TryGet(TKey key, out TValue value)
		{
			return _dic.TryGetValue(key, out value);
		}
	}
}
