using System;
using System.Collections.Generic;
using UnityEngine;

namespace WidgetUI
{
	public interface IListWidget<T>
		: IWidget<IList<T>>
		, IList<T>
	{
		Vector2 ScrollArea { get; }
		Vector2 ScrollRange { get; }
		Vector2 ScrollPosition { get; set; }
		Vector2 NormalizedScrollPosition { get; set; }
		ItemSelectEvent<T> OnSelectItem { get; set; }

		void AddRange(IEnumerable<T> p_collection);
		IList<T> Remove(Predicate<T> p_match);

		void ScrollTo(int p_index);
		void ScrollTo(T p_item);
	}
}
