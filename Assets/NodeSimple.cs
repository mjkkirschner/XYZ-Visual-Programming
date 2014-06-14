using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


public class NodeSimple : MonoBehaviour
{
		public bool HitTest (NodeSimple node_to_test, DragState state)
		{		
				// raycast from the camera through the mouse and check if we hit this current
				// node, if we do return true


				Ray ray = Camera.main.ScreenPointToRay (state._mousepos);
				var hit = new RaycastHit ();
				if (Physics.Raycast (ray, out hit)) {
						Debug.Log ("Mouse Down Hit  " + hit.collider.gameObject);

						if (hit.collider.gameObject == node_to_test.gameObject) {
								return true;
						}

				}
				return false;
		}

	

		public GameObject MyOnMouseDown (DragState current_state)
		{


				if (HitTest (this, current_state)) {
						Debug.Log ("I" + this.name + " was just clicked");
						Debug.Log (current_state);
						return this.gameObject;

				} else {
						return null;
				}
		}
}
