using UnityEngine;

namespace WidgetUI
{
	public static class RectTransformExtensions
	{
		public static void Stretch(this RectTransform p_transform)
		{
			p_transform.anchorMin = Vector2.zero;
			p_transform.anchorMax = Vector2.one;
			p_transform.offsetMin = Vector2.zero;
			p_transform.offsetMax = Vector2.zero;
		}

		public static void StretchHorizontal(this RectTransform p_transform)
		{
			p_transform.anchorMin = new Vector2(0, p_transform.anchorMin.y);
			p_transform.anchorMax = new Vector2(1, p_transform.anchorMax.y);
			p_transform.offsetMin = new Vector2(0, p_transform.offsetMin.y);
			p_transform.offsetMax = new Vector2(0, p_transform.offsetMax.y);
		}

		public static void StretchVertical(this RectTransform p_transform)
		{
			p_transform.anchorMin = new Vector2(p_transform.anchorMin.x, 0);
			p_transform.anchorMax = new Vector2(p_transform.anchorMax.x, 1);
			p_transform.offsetMin = new Vector2(p_transform.offsetMin.x, 0);
			p_transform.offsetMax = new Vector2(p_transform.offsetMax.x, 0);
		}

		public static WidgetSize GetSize(this RectTransform p_transform)
		{
			return new WidgetSize(p_transform);
		}

		public static void SetSize(this RectTransform p_transform, WidgetSize p_size)
		{
			switch(p_size.widthMode)
			{
			case WidgetSize.ResizeMode.Stretch:
				p_transform.StretchHorizontal();
				break;
			case WidgetSize.ResizeMode.Value:
				p_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, p_size.width);
				break;
			}

			switch (p_size.heightMode)
			{
			case WidgetSize.ResizeMode.Stretch:
				p_transform.StretchVertical();
				break;
			case WidgetSize.ResizeMode.Value:
				p_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, p_size.height);
				break;
			}
		}
	}
}
