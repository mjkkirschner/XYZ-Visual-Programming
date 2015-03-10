using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class dummyLayoutElement : MonoBehaviour
{
	public Bounds bounds;
	public GameObject toBound; 
	public LayoutElement layoutelement;
		// Use this for initialization
		void Start ()
		{
			if (toBound == null)
		{
			toBound = transform.parent.parent.parent.parent.GetChild(0).gameObject;
		}
			//on start calculate the renderbounds of go
			bounds = toBound.GetComponentInChildren<Renderer>().bounds;
			//now set the layout element on this

		layoutelement = this.GetComponent<LayoutElement>();
		layoutelement.preferredHeight = bounds.size.y*300;
		layoutelement.preferredWidth = bounds.size.x*100;

		}
	
		
}

