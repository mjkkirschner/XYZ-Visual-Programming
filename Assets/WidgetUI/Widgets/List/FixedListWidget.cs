using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WidgetUI
{
	public abstract class FixedListWidget<T, WidgetType>
		: Detail.ListWidgetBase<T, WidgetType, IFixedLayout>
		where WidgetType : UIBehaviour, IWidget<T>
	{
		protected Range m_renderedItems;
		protected bool m_updateView;

		protected override void Construct()
		{
			base.Construct();

			m_renderedItems = Range.Invalid;
			m_updateView = true;
			
			// setup content area
			m_contentArea.pivot = Vector2.zero;
			m_contentArea.anchorMin = Vector2.zero;
			m_contentArea.anchorMax = Vector2.zero;
			m_contentArea.offsetMin = Vector2.zero;
			m_contentArea.offsetMax = Vector2.zero;
		}

		#region Event handlers
		protected override void OnEnable()
		{
			m_scroll.onValueChanged.AddListener(this.OnScroll);
		}

		protected override void OnDisable()
		{
			m_scroll.onValueChanged.RemoveListener(this.OnScroll);
		}

		protected override void OnRectTransformDimensionsChange()
		{
			this.ScheduleViewUpdate();
		}

		protected virtual void LateUpdate()
		{
			if(m_updateView)
			{
				this.UpdateView();
				m_updateView = false;
			}
		}

		protected void OnScroll(Vector2 p_scrollPosition)
		{
			// Note: OnScroll is triggered from the ScrollRect's LateUpdate. The execution order of the LateUpdate event in this class
			// and the ScrollRect should be considered undefined. During tests LateUpdate was always called before OnScroll causing
			// the view to be desynced by one frame. That's why the view update is enforced. 
			// That means that UpdateView could be called twice a frame causing unnecessary overhead. 
			this.UpdateView();
			m_updateView = false;
		}
		#endregion

		#region Display logic
		private void ScheduleViewUpdate()
		{
			m_updateView = true;
		}

		private void UpdateView()
		{
			this.FitContentAreaSize();

			// get the indices of items that are visible
			Range visibleItems = this.GetVisibleItemIndices(m_scroll.normalizedPosition);

			// remove items that became invisible
			for (int i = m_renderedItems.Min; i < m_renderedItems.ExclusiveMax; ++i)
			{
				if (!visibleItems.Contains(i))
				{
					this.RemoveWidgetAt(i);
				}
			}

			// create items that became visible
			for (int i = visibleItems.Min; i < visibleItems.ExclusiveMax; ++i)
			{
				if (m_widgets[i] == null)
				{
					this.CreateWidgetAt(i);
				}
			}

			m_renderedItems = visibleItems;
		}

		protected void InvalidateView()
		{
			m_renderedItems.ForEach(this.RemoveWidgetAt);
			m_renderedItems = Range.Invalid;
			this.ScheduleViewUpdate();
		}

		private Rect GetViewport()
		{
			return this.GetViewport(m_scroll.normalizedPosition);
		}

		private Rect GetViewport(Vector2 p_scrollPosition)
		{
			Vector2 scrollRange = this.ScrollRange;
			Vector2 scrollArea = this.ScrollArea;
			float viewportLeft = scrollRange.x * p_scrollPosition.x;
			float viewportTop = scrollRange.y * (1.0F - p_scrollPosition.y);

			return new Rect(viewportLeft, viewportTop, scrollArea.x, scrollArea.y);
		} 

		private Range GetVisibleItemIndices(Vector2 p_scrollPosition)
		{
			int itemStartIndex, itemEndIndex;
			Rect viewport = this.GetViewport(p_scrollPosition);
			m_layout.GetVisibleWidgets(viewport, out itemStartIndex, out itemEndIndex);

			// clamp indices
			itemStartIndex = Mathf.Max(0, itemStartIndex);
			itemEndIndex = Mathf.Min(this.Count - 1, itemEndIndex);

			return new Range(itemStartIndex, itemEndIndex);
		}

		private void FitContentAreaSize()
		{
			Vector2 minSize = m_layout.GetRequiredSize(this.Count);
			Vector2 bestFit = Vector2.Max(minSize, this.ScrollArea);

			m_contentArea.offsetMax = m_contentArea.offsetMin + bestFit;
		}

		public override void ScrollTo(int p_index)
		{
			// make sure the view is up to date before applying the scroll position
			if(m_updateView)
			{
				this.UpdateView();
				m_updateView = false;
			}

			base.ScrollTo(p_index);
		}
		#endregion

		#region IListWidget implementation
		public override void Insert(int p_index, T p_item)
		{
			m_items.Insert(p_index, p_item);
			m_widgets.Insert(p_index, null);

			// if the removed item is in front of the first visible item, all positions must be recalculated
			if (p_index < m_renderedItems.Min)
			{
				++m_renderedItems.Min;
				m_renderedItems.ForEach(this.UpdateWidgetPosition);
			}
			// if the removed item was currently visible, only the following positions have to be recalculated
			else if (p_index < m_renderedItems.Max)
			{
				++m_renderedItems.Max;
				this.CreateWidgetAt(p_index);

				Range updateRange = new Range(p_index, m_renderedItems.Max);
				updateRange.ForEach(this.UpdateWidgetPosition);
			}


			this.ScheduleViewUpdate();
		}

		public override T this[int p_index]
		{
			get
			{
				return m_items[p_index];
			}
			set
			{
				m_items[p_index] = value;

				// if the widget at index is visible, update its value
				WidgetType widget = m_widgets[p_index];
				if (widget != null)
				{
					widget.Disable();
					widget.Enable(value);
				}
			}
		}

		public override void Clear()
		{
			this.InvalidateView();
			m_items.Clear();
			m_widgets.Clear();
		}

		public override void RemoveAt(int p_index)
		{
			// try to remove the rendered widget
			this.RemoveWidgetAt(p_index);

			// remove the index from the managed lists
			m_items.RemoveAt(p_index);
			m_widgets.RemoveAt(p_index);

			// if the last item has been removed, invalidate the rendered widgets range
			if(this.Count == 0)
			{
				m_renderedItems = Range.Invalid;
			}
			// if an item on before the rendered widgets was removed, all positions must be recalculated
			else if(p_index < m_renderedItems.Min)
			{
				--m_renderedItems.Min;
				m_renderedItems.ForEach(this.UpdateWidgetPosition);
			}
			// if the removed item was currently visible, only the following positions have to be recalculated
			else if (p_index < m_renderedItems.Max)
			{
				--m_renderedItems.Max;

				Range updateRange = new Range(p_index, m_renderedItems.Max);
				updateRange.ForEach(this.UpdateWidgetPosition);
			}


			this.ScheduleViewUpdate();
		}

		public override IList<T> Remove(Predicate<T> p_match)
		{
			// drop all visible widgets
			this.InvalidateView();
			return base.Remove(p_match);
		}
		#endregion
	}
}
