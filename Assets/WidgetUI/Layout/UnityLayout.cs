using System;
using UnityEngine;
using UnityEngine.UI;

namespace WidgetUI
{
	public class UnityLayout : ILayout
	{
		Transform m_transform;

		public UnityLayout(LayoutGroup p_layout)
		{
			m_transform = p_layout.transform;

#if UNITY_EDITOR
			if (m_transform.GetComponent<ContentSizeFitter>() == null)
			{
				Debug.LogWarning(String.Format("The {0} on '{1}' does not have a ContentSizeFitter component attached. You might want to check on that.", p_layout.GetType().Name, m_transform.name));
			}
#endif
		}
		
		public Vector2 GetWidgetPosition(int p_index)
		{
#if UNITY_EDITOR
			if(p_index < 0 || p_index >= m_transform.childCount)
			{
				throw new IndexOutOfRangeException("Layout does not contain a child with index: " + p_index);
			}
#endif
			
			RectTransform childTransform = (RectTransform) m_transform.GetChild(p_index);
			Vector2 position = new Vector2();
			position.x = childTransform.offsetMin.x;
			position.y = childTransform.offsetMax.y * -1;
			return position;
		}

		public void SetWidgetPosition(int p_index, RectTransform p_widgetTransform)
		{
			p_widgetTransform.SetSiblingIndex(p_index);
		}
	}
}
