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
    void Start()
    {
        m_transform = GetComponent<RectTransform>();
        
    }


    public void OnPointerDown(PointerEventData pointerdata)
    {
        dist = Vector3.Distance(m_transform.position, Camera.main.transform.position);
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (this.gameObject == eventData.pointerDrag)
        {

            m_transform.position = BaseView<BaseModel>.ProjectCurrentDrag(dist);
        }
        // magic : add zone clamping if's here.
    }
}