using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


public class NodeSimple : MonoBehaviour
{
		NodeManager NodeManager;

		void Start(){

				NodeManager = GameObject.FindObjectOfType<NodeManager> ();
		}

		public bool HitTest (NodeSimple node_to_test, DragState state)
		{		
				// raycast from the camera through the mouse and check if we hit this current
				// node, if we do return true

				Ray ray = Camera.main.ScreenPointToRay (state._mousepos);
				var hit = new RaycastHit ();
			
				Debug.DrawRay (ray.origin, ray.direction - ray.origin);
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
				Debug.Log ("event handler called");
				// check if this node was actually clicked on
				if (HitTest (this,current_state)) {
						Debug.Log ("I" + this.name + " was just clicked");
						Debug.Log (current_state);

						// check the dragstate from the GUI, either this is a connecting click
						// or a selection click
						if (current_state._connecting == false) {
								// it might be possible to eliminate the node manager selection propery
								// and just store it in the drag state.
								// we need to add this node to the current selection
								// as well as possibly update the dragstate

								NodeManager.addNodeToSelection (this);

						} else 
						{
								//a double click occured on a node
								Debug.Log ("I" + this.name + " was just DOUBLE clicked");
						}


						return this.gameObject;

						

				} else {
						return null;
				}
		}
}
