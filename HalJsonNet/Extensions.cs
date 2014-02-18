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
