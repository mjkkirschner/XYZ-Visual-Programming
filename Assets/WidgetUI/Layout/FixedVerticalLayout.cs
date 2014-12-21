using System;
using UnityEngine;

namespace WidgetUI
{
	public class FixedVerticalLayout : IFixedLayout
	{
		WidgetSize m_widgetSize;

		public WidgetSize WidgetSize
		{
			get
			{
				return m_widgetSize;
			}
			set
			{
				if (value.IsHeightStretched())
				{
					throw new ArgumentException("Widget height must be constant.");
				}
				m_widgetSize = value;
			}
		}


		public FixedVerticalLayout()
		{
		}

		public FixedVerticalLayout(WidgetSize p_widgetSize)
		{
			this.WidgetSize = p_widgetSize;
		}

		public bool SetContentAreaSize(Vector2 p_size)
		{
			return false;
		}

		public Vector2 GetWidgetPosition(int p_index)
		{
			return new Vector2(0, p_index * m_widgetSize.height);
		}

		public void SetWidgetPosition(int p_index, RectTransform p_widgetTransform)
		{
			// set widget position
			Vector2 position = this.GetWidgetPosition(p_index);
			position.y *= -1;

			p_widgetTransform.pivot = new Vector2(0, 1);
			p_widgetTransform.anchoredPosition = position;

			// resize widget
			p_widgetTransform.SetSize(this.WidgetSize);
		}


		public Vector2 GetRequiredSize(int p_widgetCount)
		{
			return new Vector2(m_widgetSize.width, p_widgetCount * m_widgetSize.height);
		}

		public void GetVisibleWidgets(Rect p_viewport, out int p_startIndex, out int p_endIndex)
		{
			p_startIndex = Mathf.FloorToInt(p_viewport.y / m_widgetSize.height);
			p_endIndex = Mathf.FloorToInt(p_viewport.yMax / m_widgetSize.height);
		}
	}
}
