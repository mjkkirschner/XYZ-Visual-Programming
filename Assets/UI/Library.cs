using UnityEngine;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using UnityEngine.UI;

public class Library : MonoBehaviour
{
	public GameObject ScrollContentPanel;
	public delegate void LibraryButtonPressEventHandler(LibraryButton sender, EventArgs e);
	public event LibraryButtonPressEventHandler ButtonPressed;

	 void Start(){
		var scrollrect = this.GetComponentInChildren<ScrollRect>().gameObject;
		ScrollContentPanel = scrollrect.transform.GetChild(0).gameObject;
	}

	/// <summary>
	/// Raises the button press event, event args will contain the type of node to instantiate.
	/// current graphmodels will subscribe to the library's buttonpress event so nodes can be instantiated
	/// </summary>
	/// <param name="e">E.</param>
	public void OnButtonPress(LibraryButton button,EventArgs e)
	{
		if (ButtonPressed != null)
			ButtonPressed(button, e);
	}


	public void HandleAppModelChanges(object appmodel,PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "LoadadNodes")
		{
			Debug.Log("populating library");
			populateLibrary(appmodel as AppModel);
		}
	}

	private List<GameObject> populateLibrary(AppModel appmodel)
	{

		var buttons = new List<GameObject>(); 
		foreach(var type in appmodel.LoadedNodeModels)
		{
			var button = Instantiate(Resources.Load("LibraryButton")) as GameObject;
			button.transform.SetParent(ScrollContentPanel.transform);
			var libbutton = button.AddComponent<LibraryButton>();
			libbutton.NameLabel.text = type.FullName;
			Debug.Log("name is" + type.FullName);
			libbutton.LoadedType = type;
			buttons.Add(button);
		}
		return buttons;
	}

}