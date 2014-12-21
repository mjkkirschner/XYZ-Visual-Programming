using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WidgetUI
{
	public class InspectableFixedListWidget<T, WidgetType>
		: FixedListWidget<T, WidgetType>
		where WidgetType : UIBehaviour, IWidget<T>
	{
		[SerializeField]
		protected WidgetType m_widgetPrefab;

		[SerializeField]
		protected ScrollRect m_scrollRectComponent;

		[SerializeField]
		protected FixedLayoutReference m_layoutClass;

		[SerializeField]
		protected WidgetSize m_elementSize;

		/// <summary>
		/// True if the Construct() method has been called.
		/// This is needed because OnRectTransformDimensionsChange() might be called 
		/// by Unity before the list is set up, causing nullpointer exceptions.
		/// </summary>
		private bool m_constructed = false;

		protected override void Construct()
		{
			if (m_widgetPrefab == null)
			{
				throw new ArgumentException(String.Format("{0}: Widget prefab must not be null", this.name));
			}

			if (m_scrollRectComponent == null)
			{
				throw new ArgumentException(String.Format("{0}: ScrollRect instance must not be null", this.name));
			}

			if (m_layoutClass.ReferencedClassType == null)
			{
				throw new ArgumentException(String.Format("{0}: No layout selected", this.name));
			}

			base.Construct();
			m_constructed = true;
		}

		protected override IWidgetAllocator<WidgetType> GetWidgetAllocator()
		{
			return new DefaultWidgetAllocator<WidgetType>(m_widgetPrefab.gameObject);
		}

		protected override IFixedLayout GetLayout()
		{
			RectTransform widgetTransformProto = m_widgetPrefab.transform as RectTransform;

			WidgetSize originalSize = widgetTransformProto.GetSize();
			if(m_elementSize.widthMode != WidgetSize.ResizeMode.Value)
			{
				m_elementSize.width = originalSize.width;
			}

			if (m_elementSize.heightMode != WidgetSize.ResizeMode.Value)
			{
				m_elementSize.height = originalSize.height;
			}

			IFixedLayout layout = (IFixedLayout)Activator.CreateInstance(m_layoutClass.ReferencedClassType);
			layout.WidgetSize = m_elementSize;
			layout.SetContentAreaSize(base.ScrollArea);

			return layout;
		}

		protected override ScrollRect GetScrollRect()
		{
			return m_scrollRectComponent;
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			if (m_constructed && base.Layout.SetContentAreaSize(base.ScrollArea))
			{
				base.InvalidateView();
			}
		}
	}
}
