using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class ExtendedFlycam : MonoBehaviour
{
 
	/*
	EXTENDED FLYCAM
		Desi Quintans (CowfaceGames.com), 17 August 2012.
		Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.
 
	LICENSE
		Free as in speech, and free as in beer.
 
	FEATURES
		WASD/Arrows:    Movement
		          Q:    Climb
		          E:    Drop
                      Shift:    Move faster
                    Control:    Move slower
                        End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
	*/
 
	public float cameraSensitivity = 200;
	public float climbSpeed = 100;
	public float normalMoveSpeed = 20;
	public float slowMoveFactor = 0.25f;
	public float fastMoveFactor = 3;
 
	private float rotationX = 0.0f;
	private float rotationY = 0.0f;
	private float panx = 0f;
	private float pany = 0f;

	void Start ()
	{
		rotationX = transform.localRotation.x;
		rotationY = transform.localRotation.y;
	}

 
	void Update ()
	{
		//cast into the scene, if we hit anything and if the item we hit is a UI tagged object
		// then do not enter input loop
		var hits = new List<RaycastResult>();
		var pointer = new PointerEventData(EventSystem.current);
		pointer.position = Input.mousePosition;
		EventSystem.current.RaycastAll(pointer,hits);
		var filterdhits = hits.Where(x=>x.gameObject.transform.root.GetComponentInChildren<ScrollRect>() != null ).ToList();
		if (filterdhits.Count>0 )
			{
				return;
			}

		// if the right mouse is clicked then rotate the camera
		if (Input.GetMouseButton(1))
		{

			rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
			rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
			rotationY = Mathf.Clamp(rotationY, -90, 90);
		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
		}
		// if the middle mouse is clicked then pan the camera
		else if (Input.GetMouseButton(2))
		{

			panx = cameraSensitivity/90 * -(Input.GetAxis("Mouse X"));
			pany = cameraSensitivity/90 * -(Input.GetAxis("Mouse Y"));
			transform.Translate(panx, pany, 0);
		}

		//check if we're on a scrollable inputfield
		var filterForScrolls = hits.Where(x=>x.gameObject.transform.GetComponent<InputFieldDebug>() != null ).ToList();
		if (filterForScrolls.Count<1 )
		{
			var scrolldelta = Input.GetAxis("Mouse ScrollWheel");
			var scrollmove = -1f* scrolldelta *cameraSensitivity/80 * transform.forward;
			transform.Translate(scrollmove, Space.World);
		}

	 	if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
	 	{
			transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
	 	}
	 	else if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl))
	 	{
			transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
	 	}
	 	else
	 	{
	 		transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
	 	}
 
 
		if (Input.GetKey (KeyCode.Q)) {transform.position += transform.up * climbSpeed * Time.deltaTime;}
		if (Input.GetKey (KeyCode.E)) {transform.position -= transform.up * climbSpeed * Time.deltaTime;}
		
	}
}