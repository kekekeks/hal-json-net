using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalJsonNet
{
	internal static class Extensions
	{
		public static string ToCamelCase (this string s)
		{
			var arr = s.ToCharArray ();
			arr[0] = char.ToLowerInvariant (arr[0]);
			return new string (arr);
		}


	}
}
