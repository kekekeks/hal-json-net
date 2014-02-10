using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HalJsonNet.Utility
{
	static class ReflectionUtility
	{
		class PropertyFinder : ExpressionVisitor
		{
			private PropertyInfo _found;
			protected override Expression VisitMember (MemberExpression node)
			{
				_found = node.Member as PropertyInfo;
				return base.VisitMember (node);
			}

			public PropertyInfo Find (Expression exp)
			{
				Visit (exp);
				return _found;
			}
		}

		public static PropertyInfo ExtractProperty (Expression exp)
		{
			return new PropertyFinder ().Find (exp);
		}


		public static T CreateDelegate<T> (MethodInfo nfo, object target)
		{
			if (target != null)
				return (T)(object)Delegate.CreateDelegate (typeof (T), target, nfo);
			return (T)(object)Delegate.CreateDelegate (typeof (T), nfo);
		}

	}
}
