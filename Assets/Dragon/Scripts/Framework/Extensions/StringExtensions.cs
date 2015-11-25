using UnityEngine;
using System.Collections;

public static class StringExtensions
{
	public static string LimitLength(this string source, int maxLength)
	{
		if (source.Length <= maxLength)
		{
			return source;
		}
		
		return source.Substring(0, maxLength);
	}
	
}
