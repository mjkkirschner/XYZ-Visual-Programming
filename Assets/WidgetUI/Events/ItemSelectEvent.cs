using System;
using UnityEngine.Events;

namespace WidgetUI
{
	[Serializable]
	public class ItemSelectEvent<T> : UnityEvent<T>
	{
	}
}
