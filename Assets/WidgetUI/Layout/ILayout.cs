using UnityEngine;

namespace WidgetUI
{
	public interface ILayout
	{

		/// <summary>
		/// Returns the position of the top left corner of an item.
		/// </summary>
		/// <remarks>
		/// Needed by IListWidget.ScrollTo
		/// </remarks>
		/// <param name="p_index">The index of the item</param>
		/// <returns>The local space 2D position</returns>
		Vector2 GetWidgetPosition(int p_index);

		/// <summary>
		/// Sets the position of a given item
		/// </summary>
		/// <param name="p_index">The index of the item</param>
		/// <param name="p_widgetTransform">The item's transform</param>
		void SetWidgetPosition(int p_index, RectTransform p_widgetTransform);

	}
}
