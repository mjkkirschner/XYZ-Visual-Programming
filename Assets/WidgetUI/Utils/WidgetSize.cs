using System;
using UnityEngine;

namespace WidgetUI
{
	[Serializable]
	public struct WidgetSize
	{
		public enum ResizeMode : byte { Original /* none; do not resize */, Value, Stretch }

		public float width, height;
		public ResizeMode widthMode, heightMode;


		public WidgetSize(RectTransform p_transform)
		{
			Rect rect = p_transform.rect;
			width = rect.width;
			height = rect.height;
			widthMode = heightMode = ResizeMode.Original;
		}

		public bool IsWidthStretched()
		{
			return (widthMode == ResizeMode.Stretch) || (width <= 0);
		}

		public bool IsHeightStretched()
		{
			return (heightMode == ResizeMode.Stretch) || (height <= 0);
		}

		public static implicit operator Vector2(WidgetSize p_size)
		{
			return new Vector2(p_size.width, p_size.height);
		}
	}
}
