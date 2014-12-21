using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WidgetUI
{
	/// <summary>
	/// A widget to select an item from a list. Similiar to a Combobox but without editing capabilities.
	/// </summary
	/// 
	/// <remarks>
	/// It consists of the following components:
	///		- A list to select an item from
	///		- An area to show the selected item
	///		- An optional button to open the list (thus no abstract getter method)
	///		
	/// The widget used to show the selected item can be different from the one used by the list.
	/// 
	/// The button used to open the list is optional. To set the button, override the 'GetPushButton' method.
	/// If no button is set, you most likely want to set the 'ClickAnywhereToOpen' property to true. 
	/// </remarks>
	/// 
	/// <typeparam name="T">The underlying datatype</typeparam>
	/// <typeparam name="WidgetType">The widget type used to display the selected item</typeparam>
	/// <typeparam name="ListWidgetType">The list widget to show the items to select from</typeparam>
	public abstract class SelectboxWidget<T, WidgetType, ListWidgetType>
		: UIBehaviour
		, IWidget<IList<T>>
		, IList<T>
		where WidgetType : UIBehaviour, IWidget<T>
		where ListWidgetType : UIBehaviour, IListWidget<T>
	{
		// abstract methods
		protected abstract IWidgetAllocator<WidgetType> GetWidgetAllocator();
		protected abstract IWidgetAllocator<ListWidgetType> GetListWidgetAllocator();
		protected abstract RectTransform GetListArea();
		protected abstract RectTransform GetSelectedItemArea();

		// members
		protected IWidgetAllocator<WidgetType> m_allocator;
		protected IWidgetAllocator<ListWidgetType> m_listAllocator;
		protected RectTransform m_listArea;
		protected RectTransform m_selectedItemArea;
		protected Button m_pushButton;

		protected ListWidgetType m_list;

		protected T m_selectedItem;
		protected WidgetType m_selectedWidget;

		protected bool m_clickAnywhereToOpen;

		protected ItemSelectEvent<T> m_onItemSelect = new ItemSelectEvent<T>();

		/// <summary>
		/// Event that fires when the user selects a new item from the list
		/// </summary>
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

		/// <summary>
		/// Gets or sets the selected item.
		/// </summary>
		public T SelectedItem
		{
			get
			{
				return m_selectedItem;
			}
			set
			{
				this.SetSelectedItem(value);
			}
		}

		/// <summary>
		/// If enabled, clicking anywhere inside the RectTransform will toggle the list.
		/// </summary>
		public bool ClickAnywhereToOpen
		{
			get
			{
				return m_clickAnywhereToOpen;
			}
			set
			{
				if(m_pushButton != null)
				{
					this.EnablePushButtonListener(!value);
				}
				m_clickAnywhereToOpen = value;
			}
		}

		/// <summary>
		/// Widget constructor
		/// </summary>
		protected virtual void Construct()
		{
			// set some default value
			m_selectedItem = default(T);
			m_clickAnywhereToOpen = false;

			// get allocators and UI elements
			m_allocator = this.GetWidgetAllocator();
			m_listAllocator = this.GetListWidgetAllocator();
			m_listArea = this.GetListArea();
			m_selectedItemArea = this.GetSelectedItemArea();
			m_pushButton = this.GetPushButton();

			// create the list and subscribe to the item selection event
			m_list = m_listAllocator.Construct();
			m_list.OnSelectItem.AddListener(this.OnItemSelected);

			// reparent the list to the list area transform provided by the inheriting class
			RectTransform listTransform = (RectTransform) m_list.transform;
			listTransform.SetParent(m_listArea, false);
			listTransform.Stretch();

			// list should be disabled by default
			m_listArea.gameObject.SetActive(false);

			// subscribe to the PushButton's click event
			this.EnablePushButtonListener(true);
		}

		/// <summary>
		/// Widget destructor
		/// </summary>
		protected virtual void Destruct()
		{
			// remove all event subscriptions
			m_list.OnSelectItem.RemoveListener(this.OnItemSelected);
			this.EnablePushButtonListener(false);

			// destroy the list and the selected item widget
			m_listAllocator.Destroy(m_list);
			if(m_selectedWidget != null)
			{
				m_allocator.Destroy(m_selectedWidget);
			}
		}

		public void Enable(IList<T> p_dataObject)
		{
			m_list.Enable(p_dataObject);
		}

		public void Disable()
		{
			this.SetListVisible(false);
			m_selectedItem = default(T);
			m_list.Disable();
		}

		/// <summary>
		/// Callback for the list's item selected event
		/// </summary>
		/// <param name="p_item">The selected item</param>
		protected virtual void OnItemSelected(T p_item)
		{
			this.SetSelectedItem(p_item);
			this.SetListVisible(false);
			m_onItemSelect.Invoke(p_item);
		}

		/// <summary>
		/// Sets the selected item and manages the visibility of the widget
		/// </summary>
		/// <param name="p_item"></param>
		protected virtual void SetSelectedItem(T p_item)
		{
			m_selectedItem = p_item;

			// If the item is set to null, make sure the widget is destroyed.
			//
			// NOTE: not checking against default(T) here because this would
			// cause strange behaviour when the type T is a value type (like int).
			// When calling this method with the parameter zero, the widget would be 
			// destroyed.
			if (m_selectedItem == null)
			{
				if (m_selectedWidget != null)
				{
					m_allocator.Destroy(m_selectedWidget);
					m_selectedWidget = null;
				}
			}
			else
			{
				// create a widget if there is none
				if (m_selectedWidget == null)
				{
					m_selectedWidget = m_allocator.Construct();
					RectTransform widgetTransform = (RectTransform)m_selectedWidget.transform;
					widgetTransform.SetParent(m_selectedItemArea, false);
					widgetTransform.Stretch();
				}
				// ... otherwise disable it. This allows the widget to clean up before passing a new value.
				else
				{
					m_selectedWidget.Disable();
				}

				// assign the value to the widget
				m_selectedWidget.Enable(m_selectedItem);
			}
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

		protected virtual void LateUpdate()
		{
			this.ProcessMouseInput();
		}

		/// <summary>
		/// Check the mouse input to capture clicks on non-graphical RectTransforms
		/// </summary>
		protected virtual void ProcessMouseInput()
		{
			// Hide the list if the mouse is pressed outside of this widget
			// or toggle the list's visibility if 'ClickAnywhereToOpen' is enabled.
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 mousePos = Input.mousePosition;
				Vector2 mouseScreenPos = new Vector2(mousePos.x, mousePos.y);

				bool clickedOwnTransform = RectTransformUtility.RectangleContainsScreenPoint(this.transform as RectTransform, mouseScreenPos, Camera.current);
				bool clickedListTransform = RectTransformUtility.RectangleContainsScreenPoint(m_listArea, mouseScreenPos, Camera.current);

				if (m_clickAnywhereToOpen && clickedOwnTransform)
				{
					this.ToggleListVisibility();
				}
				else if (!clickedOwnTransform && !clickedListTransform)
				{
					this.SetListVisible(false);
				}
			}
		}

		/// <summary>
		/// Gets the button component used to toggle the list on construction.
		/// </summary>
		/// <returns>The Button component or null if not set</returns>
		protected virtual Button GetPushButton()
		{
			return null;
		}

		/// <summary>
		/// Error prone push button event (un-)subscription
		/// </summary>
		/// <param name="p_enabled">true to add the listener, false to remove it</param>
		protected virtual void EnablePushButtonListener(bool p_enabled)
		{
			if (m_pushButton != null)
			{
				if (p_enabled)
				{
					m_pushButton.onClick.AddListener(this.OnPushButtonClick);
				}
				else
				{
					m_pushButton.onClick.RemoveListener(this.OnPushButtonClick);
				}
			}
		}

		/// <summary>
		/// Event handler for clicks on the push button
		/// </summary>
		protected virtual void OnPushButtonClick()
		{
			this.ToggleListVisibility();
		}

		/// <summary>
		/// Toggles the visibility of the list
		/// </summary>
		protected void ToggleListVisibility()
		{
			bool isVisible = m_listArea.gameObject.activeSelf;
			this.SetListVisible(!isVisible);
		}

		/// <summary>
		/// Shows or hides the list
		/// </summary>
		/// <param name="p_visible"></param>
		public virtual void SetListVisible(bool p_visible)
		{
			m_listArea.gameObject.SetActive(p_visible);
		}


		#region IList<T> implementation
		public int IndexOf(T p_item)
		{
			return m_list.IndexOf(p_item);
		}

		public void Insert(int p_index, T p_item)
		{
			m_list.Insert(p_index, p_item);
		}

		public void RemoveAt(int p_index)
		{
			m_list.RemoveAt(p_index);
		}

		public T this[int p_index]
		{
			get
			{
				throw new NotImplementedException();
				//return m_list[p_index];
			}
			set
			{
				throw new NotImplementedException();
				//m_list[p_index] = value;
			}
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return m_list.GetEnumerator();
		}

		public void Add(T p_item)
		{
			m_list.Add(p_item);
		}

		public void Clear()
		{
			m_list.Clear();
		}

		public bool Contains(T p_item)
		{
			return m_list.Contains(p_item);
		}

		public void CopyTo(T[] p_array, int p_arrayIndex)
		{
			m_list.CopyTo(p_array, p_arrayIndex);
		}

		public int Count
		{
			get { return m_list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T p_item)
		{
			return m_list.Remove(p_item);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return m_list.GetEnumerator();
		}
		#endregion
	}
}
