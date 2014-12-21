using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WidgetUI
{	
	public class InspectableListWidget<T, WidgetType>
		: ListWidget<T, WidgetType>
		where WidgetType : UIBehaviour, IWidget<T>
	{
		[SerializeField]
		protected WidgetType m_widgetPrefab;

		[SerializeField]
		protected ScrollRect m_scrollRectComponent;

		[SerializeField]
		protected LayoutGroup m_layoutGroup;

		protected override void Construct()
		{
			if(m_widgetPrefab == null)
			{
				throw new ArgumentException(String.Format("{0}: Widget prefab must not be null", this.name));
			}

			if (m_scrollRectComponent == null)
			{
				throw new ArgumentException(String.Format("{0}: ScrollRect instance must not be null", this.name));
			}

			if (m_layoutGroup == null)
			{
				throw new ArgumentException(String.Format("{0}: Layout must not be null", this.name));
			}

			base.Construct();
		}

		protected override IWidgetAllocator<WidgetType> GetWidgetAllocator()
		{
			return new DefaultWidgetAllocator<WidgetType>(m_widgetPrefab.gameObject);
		}

		protected override ILayout GetLayout()
		{
			return new UnityLayout(m_layoutGroup);
		}

		protected override ScrollRect GetScrollRect()
		{
			return m_scrollRectComponent;
		}
	}
}
