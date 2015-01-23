using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class LibraryButton : UIBehaviour, IPointerClickHandler
{
	public Button Button{get;set;}
	public Text NameLabel{get;set;}
	public Type LoadedType{get;set;}

	#region IPointerClickHandler implementation


	/// <summary>
	/// on click raise an event that a button in the library has been clicked and send position, and type of node
	/// the current graphmodel will always be subscribed to events from all library buttons... alternatively
	/// the library buttons send an event to the library, and workspace is subscribes to a single event from the
	/// library 
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick (PointerEventData eventData)
	{
		throw new NotImplementedException ();
	}

	#endregion
}

