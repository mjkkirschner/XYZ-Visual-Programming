using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class UILabel : MonoBehaviour, IPointerDownHandler
{
	RectTransform m_transform = null;
	void Start ()
	{	var model = GetComponentInParent<BaseModel>();
		m_transform = GetComponent<RectTransform> ();
		//for now temporary solution, will need other ways of setting the label to grab different text
		// or just subclas
		var castmodel = model as PortModel;
			
			if (castmodel != null)
		{
			this.GetComponentInChildren<Text>().text = castmodel.NickName;
		}
		else
		{
			this.GetComponentInChildren<Text>().text = model.name;
		}

	}

	public void OnPointerDown (PointerEventData pointerdata)
	{

	}
	
	public void Update()
	{
		m_transform.rotation = Camera.main.transform.rotation;
	}
	
	
}