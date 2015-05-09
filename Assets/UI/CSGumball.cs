using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class CSGumball : MonoBehaviour, IDragHandler
{
	Transform m_transform = null;
	GameObject x;
	GameObject y;
	GameObject z;

	void Start ()
	{	
		m_transform = GetComponent<Transform> ();
		x = transform.FindChild("X").gameObject;
		y = transform.FindChild("Y").gameObject;
		z = transform.FindChild("Z").gameObject;
		Debug.Log(x);
		Debug.Log(y);
		Debug.Log(z);
	}

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{

		var dist = Vector3.Distance(Camera.main.transform.position,this.transform.position);
		Ray ray = Camera.main.ScreenPointToRay (eventData.position - eventData.delta);
		var orginalPoint = ray.GetPoint(dist);



		if (eventData.pointerPressRaycast.gameObject == x){

			Vector3 threeddelta = BaseView<BaseModel>.ProjectCurrentDrag (dist) - orginalPoint;
			m_transform.root.transform.position += new Vector3(threeddelta.x,0,0)*2;
		}

		if (eventData.pointerPressRaycast.gameObject == y){
			Vector3 threeddelta = BaseView<BaseModel>.ProjectCurrentDrag (dist) - orginalPoint;
			m_transform.root.transform.position += new Vector3(0,threeddelta.y,0)*2;
		}

		if (eventData.pointerPressRaycast.gameObject ==z){
			Vector3 threeddelta = BaseView<BaseModel>.ProjectCurrentDrag (dist) - orginalPoint;
			m_transform.root.transform.position += new Vector3(0,0,threeddelta.z)*2;
		}
	}

	#endregion
	
	public void Update()
	{

	}
	
	
}