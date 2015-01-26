using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nodeplay.Utils
{
	static class  DictionaryHelpers
	{

		public static KeyValuePair<object, object> Cast<K, V>(this KeyValuePair<K, V> kvp)
		{
			return new KeyValuePair<object, object>(kvp.Key, kvp.Value);
		}

		public static KeyValuePair<T, V> CastFrom<T, V>(System.Object obj)
		{
			return (KeyValuePair<T, V>)obj;
		}

		public static KeyValuePair<object, object> CastFrom(System.Object obj)
		{
			var type = obj.GetType();
			if (type.IsGenericType)
			{
				if (type == typeof(KeyValuePair<,>))
				{
					var key = type.GetProperty("Key");
					var value = type.GetProperty("Value");
					var keyObj = key.GetValue(obj, null);
					var valueObj = value.GetValue(obj, null);
					return new KeyValuePair<object, object>(keyObj, valueObj);
				}
			}
			throw new ArgumentException(" ### -> public static KeyValuePair<object , object > CastFrom(Object obj) : Error : obj argument must be KeyValuePair<,>");
		}

	}
}
