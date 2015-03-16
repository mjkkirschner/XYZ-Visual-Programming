using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

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
			
			if (toBound.GetComponentInParent<ExecutionPortModel>() != null)
			{	//if we represent a execution port then dont expand the dummy
				return;
			}
			//on start calculate the renderbounds of go
			bounds = toBound.GetComponentInChildren<Renderer>().bounds;
			//now set the layout element on this

		layoutelement = this.GetComponent<LayoutElement>();
		layoutelement.preferredHeight = bounds.size.y*300;
		layoutelement.preferredWidth = bounds.size.x*300;

		}
	
		
}

