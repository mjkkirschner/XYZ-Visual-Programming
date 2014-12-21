using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WidgetUI.Detail
{
	public abstract class ListWidgetBase<T, WidgetType, LayoutType>
		: UIBehaviour
		, IListWidget<T>
		, IEnumerable<T>
		where WidgetType : UIBehaviour, IWidget<T>
		where LayoutType : ILayout
	{
		protected IWidgetAllocator<WidgetType> m_allocator;
		protected LayoutType m_layout;

		protected List<T> m_items;
		protected List<WidgetType> m_widgets;
		protected IComparer<T> m_comparer;

		protected ScrollRect m_scroll;
		protected RectTransform m_scrollTransform;
		protected RectTransform m_contentArea;

		protected ItemSelectEvent<T> m_onItemSelect = new ItemSelectEvent<T>();

		public ItemSelectEvent<T> OnSelectItem
		{
			get
			{
				return m_onItemSelect;
			}
			set
			{
				m_onItemSelect = value;
			}
		}


		protected virtual void Construct()
		{
			m_items = new List<T>();
			m_widgets = new List<WidgetType>();
			m_comparer = null;

			m_allocator = this.GetWidgetAllocator();
			
			m_scroll = this.GetScrollRect();
			m_scrollTransform = m_scroll.transform as RectTransform;

			m_layout = this.GetLayout();

			// get the ScrollRect's content area
			m_contentArea = m_scroll.content;
			if (m_contentArea == null)
			{
				GameObject contentAreaObject = new GameObject("Content");
				m_contentArea = contentAreaObject.AddComponent<RectTransform>();
				m_contentArea.SetParent(m_scroll.transform, false);
				m_scroll.content = m_contentArea;
			}
		}

		protected virtual void Destruct()
		{
		}

		protected abstract IWidgetAllocator<WidgetType> GetWidgetAllocator();
		protected abstract LayoutType GetLayout();
		protected abstract ScrollRect GetScrollRect();
		public abstract void Insert(int index, T item);
		public abstract void RemoveAt(int index);
		public abstract void Clear();
		public abstract T this[int index] { get; set; }


		public void Enable(IList<T> p_dataObject)
		{
			this.AddRange(p_dataObject);
		}

		public void Disable()
		{
			this.Clear();
		}

		protected override void Awake()
		{
			base.Awake();
			this.Construct();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.Destruct();
		}

		public LayoutType Layout
		{
			get { return m_layout; }
		}

		public Vector2 ScrollArea
		{
			get
			{
				Rect scrollRectArea = m_scrollTransform.rect;
				return new Vector2(scrollRectArea.width, scrollRectArea.height);
			}
		}

		public Vector2 ScrollRange
		{
			get
			{
				Vector2 contentSize = m_contentArea.sizeDelta;
				Vector2 scrollArea = this.ScrollArea;
				return new Vector2()
				{
					// the content size can be smaller than the visible area; don't return negative values
					x = Mathf.Max(0, contentSize.x - scrollArea.x),
					y = Mathf.Max(0, contentSize.y - scrollArea.y)
				};
			}
		}

		public Vector2 ScrollPosition
		{
			get
			{
				Vector2 normalized = this.NormalizedScrollPosition;
				Vector2 range = this.ScrollRange;
				return new Vector2(normalized.x * range.x, normalized.y * range.y);
			}
			set
			{
				Vector2 range = this.ScrollRange;
				float scrollX = value.x / range.x;
				float scrollY = (value.y - range.y) / -range.y;
				this.NormalizedScrollPosition = new Vector2(scrollX, scrollY);
			}
		}

		public Vector2 NormalizedScrollPosition
		{
			get
			{
				return m_scroll.normalizedPosition;
			}
			set
			{
				m_scroll.normalizedPosition = value;
			}
		}

		public virtual void ScrollTo(int p_index)
		{
			Vector2 widgetPosition = m_layout.GetWidgetPosition(p_index);
			this.ScrollPosition = widgetPosition;
		}

		public void ScrollTo(T p_item)
		{
			int index = this.IndexOf(p_item);
			if (index >= 0)
			{
				this.ScrollTo(index);
			}
		}

		public virtual IList<T> Remove(Predicate<T> p_match)
		{
			List<T> removeItems = new List<T>(this.Count / 2);

			for (int i = 0; i < m_items.Count; )
			{
				T item = m_items[i];
				if (p_match(item))
				{
					removeItems.Add(item);
					m_items.RemoveAt(i);
				}
				else
				{
					++i;
				}
			}

			while (m_widgets.Count > m_items.Count)
			{
				m_widgets.RemoveAt(m_widgets.Count - 1);
			}

			return removeItems;
		}

		public void Add(T p_item)
		{
			if (m_comparer == null)
			{
				this.Insert(this.Count, p_item);
			}
			else
			{
				this.AddSorted(p_item, m_comparer);
			}
		}

		private void AddSorted(T p_item, IComparer<T> p_comparer)
		{
			int index = m_items.BinarySearch(p_item, p_comparer);
			if (index < 0)
			{
				// MSDN: The zero-based index of item in the sorted List<T>, if item is found; otherwise, 
				// a negative number that is the bitwise complement of the index of the next element that 
				// is larger than item or, if there is no larger element, the bitwise complement of Count.
				index = ~index;
			}
			this.Insert(index, p_item);
		}

		public bool Remove(T p_item)
		{
			int i = m_items.IndexOf(p_item);
			if (i < 0)
			{
				return false;
			}

			this.RemoveAt(i);
			return true;
		}

		public void AddRange(IEnumerable<T> p_collection)
		{
			foreach(T item in p_collection)
			{
				this.Add(item);
			}
		}

		public int IndexOf(T p_item)
		{
			return m_items.IndexOf(p_item);
		}

		public bool Contains(T p_item)
		{
			return m_items.Contains(p_item);
		}

		public void CopyTo(T[] p_array, int p_arrayIndex)
		{
			m_items.CopyTo(p_array, p_arrayIndex);
		}

		public int Count
		{
			get { return m_items.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public IEnumerator<T> GetEnumerator()
		{
			return m_items.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}


		protected void CreateWidgetAt(int p_index)
		{
			WidgetType widget = m_allocator.Construct();

			RectTransform widgetTransform = widget.transform as RectTransform;
			widgetTransform.SetParent(m_contentArea, false);
			this.UpdateWidgetPosition(p_index, widgetTransform);

			Button button = widget.GetComponent<Button>();
			if(button != null)
			{
				button.onClick.AddListener(() => { this.OnWidgetClicked(m_items[p_index]); });
			}

			m_widgets[p_index] = widget;
			widget.Enable(m_items[p_index]);
		}

		protected void RemoveWidgetAt(int p_index)
		{
			WidgetType widget = m_widgets[p_index];
			if (widget != null)
			{
				Button button = widget.GetComponent<Button>();
				if (button != null)
				{
					button.onClick.RemoveAllListeners();
				}


				m_allocator.Destroy(widget);
				m_widgets[p_index] = null;
			}
		}

		protected void UpdateWidgetPosition(int p_index)
		{
			this.UpdateWidgetPosition(p_index, m_widgets[p_index].transform as RectTransform);
		}

		protected void UpdateWidgetPosition(int p_index, RectTransform p_transform)
		{
			m_layout.SetWidgetPosition(p_index, p_transform);
		}

		protected virtual void OnWidgetClicked(T p_item)
		{
			m_onItemSelect.Invoke(p_item);
		}
	}
}
