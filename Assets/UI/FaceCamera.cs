using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class FaceCamera : MonoBehaviour
{
	Transform m_transform = null;
	void Start ()
	{	
		m_transform = GetComponent<Transform> ();
	}
	
	public void Update()
	{
		m_transform.rotation = Camera.main.transform.rotation;
	}
	
	
}