using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WidgetUI
{
	public class DefaultWidgetAllocator<WidgetType>
		: IWidgetAllocator<WidgetType>
		where WidgetType : UIBehaviour, IWidget
	{
		GameObject m_widgetPrefab;

		public DefaultWidgetAllocator(GameObject p_widgetPrefab)
		{
#if UNITY_EDITOR
			if (p_widgetPrefab == null)
			{
				throw new ArgumentException(String.Format("Prefab for widget '{0}' must not be null", typeof(WidgetType).Name));
			}
#endif
			m_widgetPrefab = p_widgetPrefab;
		}

		public virtual WidgetType Construct()
		{
			GameObject widget = GameObject.Instantiate(m_widgetPrefab) as GameObject;
			return widget.GetComponent<WidgetType>();
		}

		public virtual void Destroy(WidgetType p_widget)
		{
			GameObject.Destroy(p_widget.gameObject);
		}
	}
}
