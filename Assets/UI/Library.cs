using UnityEngine;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using UnityEngine.UI;
using Nodeplay.Engine;
using Nodeplay.Nodes;
using System.Linq;

public class Library : MonoBehaviour
{
	public GameObject ScrollContentPanel;
	public delegate void LibraryButtonPressEventHandler(LibraryButton sender, EventArgs e);
	public event LibraryButtonPressEventHandler ButtonPressed;
	public List<GameObject> buttons = new List<GameObject>(); 
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
		if ((e.PropertyName == "LoadadNodes") || (e.PropertyName == "LoadedNodeInfos"))
		{
			Debug.Log("populating library");
			populateLibrary(appmodel as AppModel);
		}
	}

	private List<GameObject> populateLibrary(AppModel appmodel)
	{
		//TODO will need to add customnode types to the loadedNodeModels list manually in the appmodel on loading...

		foreach(var type in appmodel.LoadedNodeModels)
		{
			if(buttons.Any(x=>x.GetComponent<LibraryButton>().LoadedType == type))
			   {
				//if we already have this kind of button then dont create another one
				continue;
			}

			var button = Instantiate(Resources.Load("LibraryButton")) as GameObject;
			button.transform.SetParent(ScrollContentPanel.transform);
			var libbutton = button.AddComponent<LibraryButton>();
			libbutton.initializeButtonFromType(type);
			//add button to list of buttons
			buttons.Add(button);//}
		}
		if (appmodel.CollapsedCustomGraphNodeManager != null){
		foreach (var entry in appmodel.CollapsedCustomGraphNodeManager.NodeInfos)
		{
			if(buttons.Select(x=>x.GetComponent<LibraryButton>()).
				   OfType<CustomNodeLibraryButton>().
				   	Any(x=>x.Info.FunctionId == entry.Value.FunctionId))
			{
				//if we already have this kind of button then dont create another one
				continue;
						}
			var nodeinfo = entry.Value;
			var button = Instantiate(Resources.Load("LibraryButton")) as GameObject;
			button.transform.SetParent(ScrollContentPanel.transform);
			var libbutton = button.AddComponent<CustomNodeLibraryButton>();
			libbutton.initializeButtonFromNodeInfo(nodeinfo);
			//add button to list of buttons
			buttons.Add(button);//}
		}
		}
		return buttons;
	}

}