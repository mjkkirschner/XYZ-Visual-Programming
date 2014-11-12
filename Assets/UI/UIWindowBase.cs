using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class UIWindowBase : MonoBehaviour, IDragHandler, IPointerDownHandler
{
		RectTransform m_transform = null;
		float dist = 100;
		// Use this for initialization
		void Start ()
		{
				m_transform = GetComponent<RectTransform> ();
        
		}


		public void OnPointerDown (PointerEventData pointerdata)
		{
				dist = Vector3.Distance (m_transform.position, Camera.main.transform.position);
		}

		public void OnDrag (PointerEventData eventData)
		{
       

				//need to construct a 3d vector that we actually move the window along
				// can do this from the point on the object we dragged to, and the 

				if (this.gameObject == eventData.pointerDrag) {
						Ray ray = Camera.main.ScreenPointToRay (eventData.position - eventData.delta);
						var orginalPoint = ray.GetPoint (dist);
						Vector3 threeddelta = BaseView<BaseModel>.ProjectCurrentDrag (dist) - orginalPoint;
						m_transform.position += threeddelta;
				}
				// magic : add zone clamping if's here.
		}
}