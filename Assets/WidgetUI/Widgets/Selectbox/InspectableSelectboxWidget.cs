using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WidgetUI
{
	public class InspectableSelectboxWidget<T, WidgetType, ListWidgetType>
		: SelectboxWidget<T, WidgetType, ListWidgetType>
		where WidgetType : UIBehaviour, IWidget<T>
		where ListWidgetType : UIBehaviour, IListWidget<T>
	{
		[Tooltip("Optional")]
		[SerializeField]
		protected Button m_button;

		[SerializeField]
		protected RectTransform m_areaSelectedItem;

		[SerializeField]
		protected RectTransform m_areaList;

		[SerializeField]
		protected WidgetType m_selectedItemWidget;

		[SerializeField]
		protected ListWidgetType m_itemList;

		protected override void Construct()
		{
			if (m_areaSelectedItem == null)
			{
				throw new ArgumentException(String.Format("{0}: Selected item area must not be null", this.name));
			}

			if (m_areaList == null)
			{
				throw new ArgumentException(String.Format("{0}: List area must not be null", this.name));
			}

			if (m_itemList == null)
			{
				throw new ArgumentException(String.Format("{0}: List must not be null", this.name));
			}

			if (m_selectedItemWidget == null)
			{
				throw new ArgumentException(String.Format("{0}: Selected item widget must not be null", this.name));
			}

			base.Construct();
		}

		protected override IWidgetAllocator<WidgetType> GetWidgetAllocator()
		{
			return new DefaultWidgetAllocator<WidgetType>(m_selectedItemWidget.gameObject);
		}

		protected override IWidgetAllocator<ListWidgetType> GetListWidgetAllocator()
		{
			return new DefaultWidgetAllocator<ListWidgetType>(m_itemList.gameObject);
		}

		protected override RectTransform GetListArea()
		{
			return m_areaList;
		}

		protected override RectTransform GetSelectedItemArea()
		{
			return m_areaSelectedItem;
		}

		protected override Button GetPushButton()
		{
			// if there's no button, open the list by clicking anywhere on the own transform
			if (m_button == null)
			{
				base.ClickAnywhereToOpen = true;
			}
			return m_button;
		}
	}

}
