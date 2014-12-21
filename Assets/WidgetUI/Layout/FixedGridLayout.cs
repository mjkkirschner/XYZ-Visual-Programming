using System;
using UnityEngine;

namespace WidgetUI
{
	public class FixedGridLayout : IFixedLayout
	{
		WidgetSize m_widgetSize;
		int m_itemsPerRow;

		public WidgetSize WidgetSize
		{
			get
			{
				return m_widgetSize;
			}
			set
			{
				if (value.IsWidthStretched())
				{
					throw new ArgumentException("Widget width must be constant.");
				}
				if (value.IsHeightStretched())
				{
					throw new ArgumentException("Widget height must be constant.");
				}
				m_widgetSize = value;
			}
		}

		public int ItemsPerRow
		{
			get
			{ 
				return m_itemsPerRow;
			}
		}

		public FixedGridLayout()
		{
		}

		public FixedGridLayout(WidgetSize p_widgetSize, Vector2 p_scrollAreaSize)
		{
			this.WidgetSize = p_widgetSize;
			this.SetContentAreaSize(p_scrollAreaSize);
		}

		public bool SetContentAreaSize(Vector2 p_size)
		{
			int previousItemsPerRow = m_itemsPerRow;
			m_itemsPerRow = Mathf.FloorToInt(p_size.x / m_widgetSize.width);
			if(m_itemsPerRow <= 0)
			{
				m_itemsPerRow = 1;
			}

			return previousItemsPerRow != m_itemsPerRow;
		}

		private int GetIndex(int p_x, int p_y)
		{
			return p_x + p_y * m_itemsPerRow;
		}

		private void GetIndex2D(int p_index, out int p_x, out int p_y)
		{
			p_x = p_index % m_itemsPerRow;
			p_y = p_index / m_itemsPerRow;
		}

		public Vector2 GetWidgetPosition(int p_index)
		{
			int x, y;
			this.GetIndex2D(p_index, out x, out y);

			return new Vector2(x * m_widgetSize.width, y * m_widgetSize.height);
		}

		public void SetWidgetPosition(int p_index, RectTransform p_widgetTransform)
		{
			Vector2 position = this.GetWidgetPosition(p_index);
			position.y *= -1;

			p_widgetTransform.pivot = new Vector2(0, 1);
			p_widgetTransform.anchorMin = new Vector2(0, 1);
			p_widgetTransform.anchorMax = new Vector2(0, 1);
			p_widgetTransform.anchoredPosition = position;

			// resize widget
			p_widgetTransform.SetSize(this.WidgetSize);
		}


		public Vector2 GetRequiredSize(int p_widgetCount)
		{
			int x, y;
			this.GetIndex2D(p_widgetCount, out x, out y);

			if (p_widgetCount > m_itemsPerRow)
			{
				x = m_itemsPerRow;
			}

			if (p_widgetCount % m_itemsPerRow != 0)
			{
				++y;
			}

			return new Vector2(x * m_widgetSize.width, y * m_widgetSize.height);
		}


		public void GetVisibleWidgets(Rect p_viewport, out int p_startIndex, out int p_endIndex)
		{
			int y1 = Mathf.FloorToInt(p_viewport.yMin / m_widgetSize.height);
			int y2 = Mathf.FloorToInt(p_viewport.yMax / m_widgetSize.height);

			p_startIndex = this.GetIndex(0, y1);
			p_endIndex = this.GetIndex(m_itemsPerRow - 1, y2);
		}

	}
}
