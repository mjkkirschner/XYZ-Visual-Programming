using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class GuiTest:MonoBehaviour
{

		public static List<DragState> statelist = new List<DragState> ();

		// GuiController generates new ui states based on previous state
		// and current event

		//define delegates
		// will need to revist these.
		public delegate DragState Mouse_Down (DragState currentstate);
		//maybe on click
		public delegate DragState Mouse_Up (DragState currentstate);

		public delegate DragState Mouse_Drag (DragState currentstate);

		//define our events

		public event Mouse_Down onMouseDown;
		public event Mouse_Up onMouseUp;
		public event Mouse_Drag onMouseDrag;

		public List<NodeSimple> nodes_to_notify = new List<NodeSimple> ();

		public bool connecting ()
		{
		
				return statelist.Last ()._connecting;
		}

		void Start ()
		{		
				// collect all nodes to 

				nodes_to_notify = new List<NodeSimple> (GameObject.FindObjectsOfType<NodeSimple> ());

				// add all nodes as listeners to the mouse downevent
				foreach (NodeSimple node in nodes_to_notify) {

						this.onMouseDown += new Mouse_Down (node.MyOnMouseDown);
						this.onMouseUp += new Mouse_Up (node.MyOnMouseUp);

				}
		}


		void Update ()
		{
				// must be something better than this, whenever a new node is created it needs to add itself to some list so we dont need to do this
				// each frame, possibly on the node manager.
				nodes_to_notify = new List<NodeSimple> (GameObject.FindObjectsOfType<NodeSimple> ());


		}



		public List<NodeSimple> CurrentSelection ()
		{
				if (statelist.Count > 0) {
						return statelist.Last ().selection;
				} else {

						return new List<NodeSimple> ();
				}
		}

		public static Vector3 ProjectCurrentDrag (float distance)
		{

				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				var output = ray.GetPoint (distance);
				return output;
		}


		public static Vector3 HitPosition (Node node_to_test)
		{

				// return the coordinate in world space where hit occured

				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				var hit = new RaycastHit ();
				if (Physics.Raycast (ray, out hit)) {
						Debug.Log ("Mouse Down Hit " + hit.collider.gameObject);

						if (hit.collider.gameObject == node_to_test.gameObject) {
								return hit.point;
								// I was previoulsy returning hit.barycenter ... triangle?
						}

				}
				return node_to_test.transform.position;
		}











		public void OnGUI ()
		{

				//Debug.Log (selection);
				//Debug.Log (connecting);

				switch (Event.current.type) {
				case EventType.mouseDown:

						// generate a new state with the current mouse position 
						//var currentSel = CurrentSelection ();
						var currentSel = new List<NodeSimple> ();
						var state = new DragState (false, false, Event.current.mousePosition, currentSel);
						statelist.Add (state);
						Debug.Log (state);

						if (onMouseDown != null) {
								Debug.Log ("sending event onmousedown");
								onMouseDown (state);
						}

						if (Event.current.clickCount == 2) {// If we double-clicked it, enter connect mode

								// current selection should be the node we single clicked on previously
								currentSel = CurrentSelection ();
								state = new DragState (true, false, Event.current.mousePosition, currentSel);
								statelist.Add (state);
								//Debug.Log (state);

								if (onMouseDown != null) {
										// should this really just be generating a different event on the node?
										Debug.Log ("calling doubleclick function");
										onMouseDown (state);
								}


						}

						Event.current.Use ();

						break;
				case EventType.mouseUp:
						// If we released the mouse button...
						// ... with no active selection, ignore the event
						// all these current selection checks might as well go into 
						// the last dragstate and check the current selection there
						// instead of even keeping any properties on the node manager.
						// refactor that out if this works for connecting variable
					

						if (CurrentSelection ().Count == 0) {				
								Debug.Log ("ignoring event");
								state = new DragState (false, false, Event.current.mousePosition, new List<NodeSimple> ());
								statelist.Add (state);
								Debug.Log ("calling mouseup function, this was an ignored event");
								if (onMouseUp != null) {
										// not sure about this dragstate

										onMouseUp (state);

								}


										// ... while some node was active selection
										// then we need to clear the selection
								} else if (CurrentSelection ().Count > 0) {


										if (!connecting ()) {					// ... and we were not in connect mode, clear the selection

												Debug.Log ("not connecting mouse up");
												// clear the selection, and create a new state
												state = new DragState (false, false, Event.current.mousePosition, new List<NodeSimple> ());
												statelist.Add (state);
												Event.current.Use ();
												if (onMouseUp != null) {
														onMouseUp (state);
												}
												break;
												// send the mouseup event

										
										}


						
								}
								
								break;

















//				case EventType.mouseDrag:
//						if (selection == this)
//						{				// If doing a mouse drag with this component selected...
//								if (connecting)
//								{					// ... and in connect mode, just use the event as we'll be painting the new connection
//										Debug.Log ("connecting");
//										Event.current.Use ();
//								} else
//								{					// ... and not in connect mode, drag the component// since we are in 3d space now, we need to conver this to a vector3...
//										// for now just use the z coordinate of the first object
//
//										// get the hit world coord
//										//var pos = HitPosition(this);
//										// calculate distance between hit loc and camera
//										//var dist_to_camera = Vector3.Distance (this.transform.position, Camera.main.transform.position); 
//
//										// project from camera through mouse currently and use same distance
//										Vector3 to_point = ProjectCurrentDrag (dist_to_camera);
//
//										// move object to new coordinate
//										this.gameObject.transform.position = to_point;
//										//this.gameObject.transform.position -= new Vector3(Event.current.delta.x/(Screen.width*(dist_to_camera)),Event.current.delta.y/(Screen.height*(dist_to_camera)),0);
//										Event.current.Use ();
//								}
//						}
//						break;
//				case EventType.repaint:
//						//GUI.skin.box.Draw (nodeRect, new GUIContent (name), false, false, false, false);
//						// The component box
//
//						if (selection == this && connecting)
//						{				// The new connection
//
//								// logic not working exactly right at the time of mouse up
//								foreach (var line in lines) {
//										Destroy (line);
//
//								}
//								lines.Clear ();
//
//
//								var pos = HitPosition (this);
//								var dist_to_camera = Vector3.Distance (pos, Camera.main.transform.position); 
//								Vector3 to_point = ProjectCurrentDrag (dist_to_camera);
//
//								var currentline = NodeManager.DrawLine (this.gameObject.transform.position, to_point);
//								this.lines.Add (currentline);
//
//
//
//						}
//
//						break;
//				}
//		}
//
//		public void DrawLine (LineRenderer line, Vector3 from, Vector3 to)
//		{
//
//				line.SetWidth (.1f, .1f);
//				line.SetVertexCount (2);
//				//line.material = aMaterial;
//				line.renderer.enabled = true;
//				line.SetPosition (0, from);
//				line.SetPosition (1, to);
//
//
//		}
//
//
//
//		public static void DrawConnection (Vector3 from, Vector2 to)
//		// Render a node connection between the two given points
//		{
//				bool left = from.x > to.x;
//
//				Handles.DrawBezier (
//						new Vector3 (from.x + (left ? -kNodeSize : kNodeSize) * 0.5f, from.y, 0.0f),
//						new Vector3 (to.x + (left ? kNodeSize : -kNodeSize) * 0.5f, to.y, 0.0f),
//						new Vector3 (from.x, from.y, 0.0f) + Vector3.right * 50.0f * (left ? -1.0f : 1.0f),
//						new Vector3 (to.x, to.y, 0.0f) + Vector3.right * 50.0f * (left ? 1.0f : -1.0f),
//						GUI.color,
//						null,
//						2.0f
//				);
//		}

				}

		}

}