using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


public class GuiTest:MonoBehaviour
{	

		// GuiController generates new ui states based on previous state
		// and current event


		//define delegates
		// will need to revist these.
		public delegate GameObject Mouse_Down(DragState currentstate); //maybe on click
		delegate GameObject Mouse_Up(DragState currentstate);
		delegate void Mouse_Drag(DragState currentstate);

		//define our events

		public event Mouse_Down onMouseDown;

		public List<NodeSimple> nodes_to_notify = new List<NodeSimple> ();

		void Start ()
		{
				nodes_to_notify = new List<NodeSimple> (GameObject.FindObjectsOfType<NodeSimple> ());

				foreach (NodeSimple node in nodes_to_notify) {

						this.onMouseDown += new Mouse_Down(node.MyOnMouseDown);

				}
		}


		void Update ()
		{
				// must be something better than this, whenever a new node is created it needs to add itself to some list so we dont need to do this
				// each frame.
				nodes_to_notify = new List<NodeSimple> (GameObject.FindObjectsOfType<NodeSimple> ());


		}


		static Node selection = null;
		static bool connecting = false;
		float dist_to_camera;

		string name;
		public List<Node> targets = new List<Node> ();

		public List<GameObject> lines = new List<GameObject> ();


		public static Node Selection {
				get {
						Debug.Log (selection);
						return selection;
				}
				set {
						selection = value;
						Debug.Log (selection);
						if (selection == null) {
								connecting = false;
						}
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

		public bool HitTest (Node node_to_test)
		{		
				// raycast from the camera through the mouse and check if we hit this current
				// node, if we do return true


				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				var hit = new RaycastHit ();
				if (Physics.Raycast (ray, out hit)) {
						Debug.Log ("Mouse Down Hit  " + hit.collider.gameObject);

						if (hit.collider.gameObject == node_to_test.gameObject) {
								return true;
						}

				}
				return false;
		}

		public ReadOnlyCollection<Node> Targets {
				get {
						return targets.AsReadOnly ();
				}
		}

		public void removeTarget (Node target)
		{
				if (targets.Contains (target)) {
						targets.Remove (target);
				}

				return;
		}


		public void ConnectTo (Node target)
		{
				if (targets.Contains (target)) {
						return;
				}

				targets.Add (target);
		}


		public void OnGUI ()
		{

				//Debug.Log (selection);
				//Debug.Log (connecting);

				switch (Event.current.type) {
				case EventType.mouseDown:
							
						var state = new DragState (false, false, Event.current.mousePosition,// this needs to be  list of simple nodes);
						Debug.Log (state);
						if (onMouseDown != null) {
								Debug.Log("calling function");
								onMouseDown (state);
						}

								if (Event.current.clickCount == 2) {					// If we double-clicked it, enter connect mode
										connecting = true;
								}

								Event.current.Use ();

						break;
//				case EventType.mouseUp:
//						// If we released the mouse button...
//						if (selection == null) {				// ... with no active selection, ignore the event
//								Debug.Log ("ignoring event");
//								break;
//						} else if (selection == this) {				// ... while this node was active selection...
//								if (!connecting) {					// ... and we were not in connect mode, clear the selection
//										Debug.Log ("not connecting mouse up");
//										Selection = null;
//										Event.current.Use ();
//								}
//						} else if (connecting && HitTest (this)) {				// ... over this component while in connect mode, connect selection to this node and clear selection
//								Debug.Log ("connecting mouse up");
//								selection.ConnectTo (this);
//								Selection = null;
//								Event.current.Use ();
//						} else if (connecting && !HitTest (this)) {
//
//								Debug.Log ("need to destroy line");
//								targets.Clear ();
//
//
//						}
//
//
//						break;
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