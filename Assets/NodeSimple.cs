using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


public class NodeSimple : MonoBehaviour
{
		NodeManager NodeManager;

		DragState GeneratedDragState;

		public List<NodeSimple> targets = new List<NodeSimple> ();

		void Start(){

				NodeManager = GameObject.FindObjectOfType<NodeManager> ();
		}

		public ReadOnlyCollection<NodeSimple> Targets {
				get {
						return targets.AsReadOnly ();
				}
		}

		public void removeTarget (NodeSimple target)
		{
				if (targets.Contains (target)) {
						targets.Remove (target);
				}

				return;
		}


		public void ConnectTo (NodeSimple target)
		{
				if (targets.Contains (target)) {
						return;
				}

				targets.Add (target);
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

		public DragState MyOnMouseUp(DragState current_state){
				// if we're connecting to this node, then add this node
				// to the target list of each of the nodes in the selection.

				Debug.Log ("Mouse up even handler called");

				if (current_state._connecting && HitTest (this, current_state)) {
						foreach (NodeSimple node in current_state.selection) {
								node.ConnectTo (this);
								Debug.Log("added" +  this.ToString() + "to" + node.ToString() + "target list");
						}

				}


				else if (current_state._connecting && !HitTest(this,current_state)){
						Debug.Log("need to destroy line, mouseup while connecting, but not over a node");
						targets.Clear();
				}

				var newState = new DragState (false, false, current_state._mousepos, new List<NodeSimple> ());
				return newState;


		}

		

		public DragState MyOnMouseDown (DragState current_state)
		{
				Debug.Log ("mouse down event handler called");
				// check if this node was actually clicked on
				if (HitTest (this,current_state)) {
						Debug.Log ("I" + this.name + " was just clicked");
						Debug.Log (current_state);

						// check the dragstate from the GUI, either this is a connecting click
						// or a selection click
						if (current_state._connecting == false) {

								// add this node to the current selection
								// update the drag state
								// store this drag state in the list of all dragstates

								List<NodeSimple> new_sel = (new List<NodeSimple> (current_state.selection));
								new_sel.Add (this);
								var newState = new DragState (current_state._connecting, current_state._dragging, current_state._mousepos, new_sel); 
								GuiTest.statelist.Add (newState);
								GeneratedDragState = newState;


						} else 
						{
								//a double click occured on a node
								Debug.Log ("I" + this.name + " was just DOUBLE clicked");

						}

						// finally return the new dragstate(not what this function should return)
						// since we actually have the dragstate stored
						// it might make more sense not to allow the Node to 
						// store the state on the GUI...
						return GeneratedDragState;

						

				} else {
						return null;
				}
		}
}
