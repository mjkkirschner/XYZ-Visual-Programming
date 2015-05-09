using UnityEngine;
using System.Collections;
using Nodeplay.Core;
using Nodeplay.Engine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForwardEventsToCurrentGraph : MonoBehaviour,IPointerClickHandler,IPointerDownHandler {


	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		var graph = GameObject.FindObjectOfType<AppModel>().GetCurrentGraphModel();
		graph.OnPointerClick(eventData);
	}

	#endregion

	#region IPointerDownHandler implementation
	public void OnPointerDown (PointerEventData eventData)
	{
		var graph = GameObject.FindObjectOfType<AppModel>().GetCurrentGraphModel();
		graph.OnPointerDown(eventData);
	}
	#endregion
}
