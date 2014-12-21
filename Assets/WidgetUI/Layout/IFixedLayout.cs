using UnityEngine;

namespace WidgetUI
{
	public interface IFixedLayout
		: ILayout
	{
		WidgetSize WidgetSize { get; set; }

		bool SetContentAreaSize(Vector2 p_size);

		Vector2 GetRequiredSize(int p_widgetCount);

		void GetVisibleWidgets(Rect p_viewport, out int p_startIndex, out int p_endIndex);
	}
}
