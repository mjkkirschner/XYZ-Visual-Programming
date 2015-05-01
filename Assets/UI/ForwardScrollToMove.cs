using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;

public class ForwardScrollToMove : MonoBehaviour,IScrollHandler {

	private InputFieldDebug inf;
	// Use this for initialization
	void Start () 
	{
		inf = this.GetComponent<InputFieldDebug>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	private void reflectInvokePrivate (string methodName)
	{

	MethodInfo dynMethod = inf.GetType().GetMethod(methodName,
		                                               BindingFlags.NonPublic | BindingFlags.Instance);
	dynMethod.Invoke(inf, new object[] {});
	
	}
	#region IScrollHandler implementation

	public void OnScroll (PointerEventData eventData)
	{
		if (eventData.IsScrolling())
		{
			if (eventData.scrollDelta.y < 0)
			{
				var ev = Event.KeyboardEvent("up");
				inf.ProcessEvent(ev);
				reflectInvokePrivate("UpdateLabel");
			}
			else{
				var ev = Event.KeyboardEvent("down");
				inf.ProcessEvent(ev);
				reflectInvokePrivate("UpdateLabel");

			}
		}
	}
	#endregion
}
