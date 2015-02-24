using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class LibraryButton : UIBehaviour, IPointerClickHandler
{
	public Button Button{get;set;}
	public Text NameLabel{get;set;}
	public Type LoadedType{get;set;}
	private Library _nodelibrary {get;set;}

	 protected override void OnEnable()
	{	//find the library that is somewhere above the library button
		_nodelibrary = this.transform.root.GetComponentInChildren<Library>();
		if (_nodelibrary == null)
		{
			_nodelibrary = GameObject.FindObjectOfType<Library>();
		}

		Button = this.GetComponent<Button>();
		NameLabel = this.GetComponentInChildren<Text>();

	}
	public virtual void initializeButtonFromType(Type type){
		this.NameLabel.text = type.FullName;
		Debug.Log("name is" + type.FullName);
		this.LoadedType = type;
	}

	#region IPointerClickHandler implementation


	/// <summary>
	/// on click raise an event that a button in the library has been clicked and send position, and type of node
	/// the current graphmodel will always be subscribed to events from all library buttons... alternatively
	/// the library buttons send an event to the library, and workspace subscribes to a single event from the
	/// library 
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick (PointerEventData eventData)
	{
		_nodelibrary.OnButtonPress(this,EventArgs.Empty);
	}

	#endregion



}

