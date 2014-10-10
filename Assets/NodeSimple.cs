using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class NodeSimple : MonoBehaviour
{
		NodeManager NodeManager;
		DragState GeneratedDragState;
		//possibly we store a list of connectors that we keep updated
		// from ports, will need to add events on ports
		public List<GameObject> connectors = new List<GameObject> ();
		
		//variable to help situate projection of mousecoords into worldspace
		private float dist_to_camera;

		void Start ()
		{
				// nodemanager manages nodes - like a workspacemodel
				NodeManager = GameObject.FindObjectOfType<NodeManager> ();
				// guimanager - like a GUIcontroller
				var GuiManager = GameObject.FindObjectOfType<GuiTest> ();

				GuiManager.onMouseDown += new GuiTest.Mouse_Down (this.MyOnMouseDown);
				GuiManager.onMouseUp += new GuiTest.Mouse_Up (this.MyOnMouseUp);
				GuiManager.onMouseDrag += new GuiTest.Mouse_Drag (this.MyOnMouseDrag);
				GuiManager.onGuiRepaint += new GuiTest.GuiRepaint (this.onGuiRepaint);
		}

		// node should not know about targets or connecting etc, this is for ports to
		// implement

		/*
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
				if (targets.Contains (target) || target == this) {
						return;
				}

				targets.Add (target);
		}
*/
		public static Vector3 ProjectCurrentDrag (float distance)
		{

				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				var output = ray.GetPoint (distance);
				return output;
		}

		
		public Vector3 HitPosition (NodeSimple node_to_test)
		{

				// return the coordinate in world space where hit occured

				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				var hit = new RaycastHit ();
				if (Physics.Raycast (ray, out hit)) {
						Debug.Log ("Mouse Down Hit " + hit.collider.gameObject);
						Debug.DrawRay (ray.origin, ray.direction * dist_to_camera, Color.red, 2.0f);
						if (hit.collider.gameObject == node_to_test.gameObject) {
								return hit.point;
								// I was previoulsy returning hit.barycenter ... triangle?
						}

				}
				return node_to_test.transform.position;
		}
		/// <summary>
		/// check if we hit the current node to test, use the state to extract mouseposition
		/// </summary>
		/// <returns><c>true</c>, if test was hit, <c>false</c> otherwise.</returns>
		/// <param name="node_to_test">Node_to_test.</param>
		/// <param name="state">State.</param>
		public bool HitTest (NodeSimple node_to_test, DragState state)
		{		
				// raycast from the camera through the mouse and check if we hit this current
				// node, if we do return true

				Ray ray = Camera.main.ScreenPointToRay (state._mousepos);
				var hit = new RaycastHit ();
			

				if (Physics.Raycast (ray, out hit)) {
						Debug.Log ("Mouse Down Hit  " + hit.collider.gameObject);
						Debug.DrawRay (ray.origin, ray.direction * dist_to_camera);
						if (hit.collider.gameObject == node_to_test.gameObject) {
								return true;
						}

				}
				return false;
		}

		public DragState MyOnMouseUp (DragState current_state)
		{
				// if we're connecting to this node, then add this node
				// to the target list of each of the nodes in the selection.

				Debug.Log ("Mouse up event handler called");
				//Debug.Log (current_state);
				//Debug.Log (this);

				if (current_state._connecting && HitTest (this, current_state)) {
						foreach (NodeSimple node in current_state.selection) {
								node.ConnectTo (this);
								Debug.Log ("added" + this.ToString () + "to" + node.ToString () + "target list");
						}
						// TODO this logic no longer makes sense, all nodes recieve this event
						// so they end up deleting their targets on any connection.
						// we can either check all invocators and check all are null like before
						// or the alternative is different logic here... It's best
						// if the logic for this is another event, GUI should handle the case
						// where we mouse up over the canvas and send the appropriate event and dragstate.

						// quick fix is to only clear the current selections targets...still broken...
				} else if (current_state._connecting && !HitTest (this, current_state) && current_state.selection.Contains (this)) {
						
						Debug.Log ("need to destroy line, mouseup while connecting, but not over a node");
						Debug.Log (current_state);
						targets.Clear ();
				}
				

				var newState = new DragState (false, false, current_state._mousepos, new List<NodeSimple> ());
				return newState;


		}

		public DragState MyOnMouseDrag (DragState current_state)
		{
				DragState newstate = current_state;
				Debug.Log ("drag even handler");

				if (current_state.selection.Contains (this)) {				// If doing a mouse drag with this component selected...
						if (current_state._connecting) {					// ... and in connect mode, just use the event as we'll be painting the new connection

								newstate = new DragState (true, false, Input.mousePosition, current_state.selection);
								Debug.Log ("connecting");
								Event.current.Use (); 
						} else {					// ... and not in connect mode, drag the component// since we are in 3d space now, we need to conver this to a vector3...
								// for now just use the z coordinate of the first object
						
								// get the hit world coord
								var pos = HitPosition (this);
								// calculate distance between hit loc and camera
								//var dist_to_camera = Vector3.Distance (this.transform.position, Camera.main.transform.position); 
						
								// project from camera through mouse currently and use same distance
								Vector3 to_point = ProjectCurrentDrag (dist_to_camera);
						
								// move object to new coordinate
								this.gameObject.transform.position = to_point;
								//this.gameObject.transform.position -= new Vector3(Event.current.delta.x/(Screen.width*(dist_to_camera)),Event.current.delta.y/(Screen.height*(dist_to_camera)),0);
								newstate = new DragState (false, true, Input.mousePosition, current_state.selection);

								Event.current.Use ();
						}
				}
				return newstate;


		}

		public DragState MyOnMouseDown (DragState current_state)
		{
				Debug.Log ("mouse down event handler called");
				// check if this node was actually clicked on
				if (HitTest (this, current_state)) {
						Debug.Log ("I" + this.name + " was just clicked");
						dist_to_camera = Vector3.Distance (this.transform.position, Camera.main.transform.position);

						Debug.Log (current_state);

						// check the dragstate from the GUI, either this is a connecting click
						// or a selection click
						// or possibly a click on nothing
						if (current_state._connecting == false) {

								// add this node to the current selection
								// update the drag state
								// store this drag state in the list of all dragstates

								List<NodeSimple> new_sel = (new List<NodeSimple> (current_state.selection));
								new_sel.Add (this);
								var newState = new DragState (current_state._connecting, current_state._dragging, current_state._mousepos, new_sel); 
								GuiTest.statelist.Add (newState);
								GeneratedDragState = newState;


						} else {
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

		public void onGuiRepaint ()
		{

		}

}
