using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

	public static class ListHelper
	{
		public static string TocommaString<T>(this List<T> list)
		{
		// work on this
		// it wants to return the list of T but we want strings... hmm
		//return list.Aggregate((a, x) => a.ToString() + x.ToString());
		var strings = list.ConvertAll<string>(x => x.ToString());
		//Debug.Log(strings);
		var result = string.Join(", ", strings.ToArray());
		return result;
		}
		
	
	}


