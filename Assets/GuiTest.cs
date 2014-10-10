﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class GuiTest:MonoBehaviour
{
		public NodeManager NManager;
		public static List<DragState> statelist = new List<DragState> ();

		// GuiController generates new ui states based on previous state
		// and current event

		//define delegates
		// will need to revist these.
		public delegate DragState Mouse_Down (DragState currentstate);
		//maybe on click
		public delegate DragState Mouse_Up (DragState currentstate);

		public delegate DragState Mouse_Drag (DragState currentstate);

		public delegate DragState Canvas_Double_Click (DragState currentstate);
		
		public delegate void GuiRepaint ();

		//define our events

		public event Mouse_Down onMouseDown;
		public event Mouse_Up onMouseUp;
		public event Mouse_Drag onMouseDrag;
		public event Canvas_Double_Click onCanvasDoubleClick;
		public event GuiRepaint onGuiRepaint;

		public List<NodeSimple> nodes_to_notify = new List<NodeSimple> ();
		
		/// <summary>
		/// returns the most recent state's connecting bool
		/// </summary>
		public bool connecting ()
		{
		
				return statelist.Last ()._connecting;
		}
		
		/// <summary>
		/// Returns the most recent states's selection list 
		/// </summary>
		/// <returns>The selection.</returns>
		public List<NodeSimple> CurrentSelection ()
		{
				if (statelist.Count > 0) {
						return statelist.Last ().selection;
				} else {
			
						return new List<NodeSimple> ();
				}
		}
		/// <summary>
		/// initialization of nodes, node manager, and adds nodes as listeners to events.   
		/// </summary>
		void Start ()
		{		
				// collect all nodes to 
				NManager = GameObject.FindObjectOfType<NodeManager> ();
				nodes_to_notify = new List<NodeSimple> (GameObject.FindObjectsOfType<NodeSimple> ());

				// add all nodes as listeners to the mouse downevent
				//	foreach (NodeSimple node in nodes_to_notify) {
				//
				//				this.onMouseDown += new Mouse_Down (node.MyOnMouseDown);
				//				this.onMouseUp += new Mouse_Up (node.MyOnMouseUp);
				//				this.onMouseDrag += new Mouse_Drag (node.MyOnMouseDrag);
				//				this.onGuiRepaint += new GuiRepaint (node.onGuiRepaint);

				//		}

				this.onCanvasDoubleClick += new Canvas_Double_Click (NManager.onCanvasDoubleClick);
				this.onGuiRepaint += new GuiRepaint (NManager.onGuiRepaint);
		}
		

		//TODO remove the need for this code by subscribing node to needed events at creation time
		void Update ()
		{
				// must be something better than this, whenever a new node is created it needs to add itself to some list so we dont need to do this
				// each frame, possibly on the node manager.
				nodes_to_notify = new List<NodeSimple> (GameObject.FindObjectsOfType<NodeSimple> ());


		}
		/// <summary>
		/// Raises the GUI events and creates states that reflect the state of the UI.
		/// </summary>
		public void OnGUI ()
		{

				if (DragState.selection_changed (statelist)) {
						Debug.Log ("selection has changed");
						Debug.Log (CurrentSelection ().TocommaString ());

				
				}

				


				Debug.Log (CurrentSelection ().TocommaString ());
				//Debug.Log (connecting);

				switch (Event.current.type) {
				case EventType.mouseDown:

						// generate a new state with the current mouse position 
						
						var currentSel = new List<NodeSimple> ();
						var state = new DragState (false, false, Event.current.mousePosition, currentSel);
						statelist.Add (state);
						//Debug.Log (state);

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
										var results = new List<DragState> ();
										// should this really just be generating a different event on the node?
										Debug.Log ("calling doubleclick function");
										foreach (Mouse_Down d in onMouseDown.GetInvocationList()) {
												results.Add (d (state));

										}
										if (results.All (element => element == null)) {
												Debug.Log ("just double clicked to the canvas, all nodes returned null");
												Debug.Log ("should create new node");
												// Send a different event
												// One that the nodemanager subscribes to
												if (onCanvasDoubleClick != null) {
														onCanvasDoubleClick (state);
												}
										}
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

										
								} else {
										// we are connecting
										// send a similar state but with connecting true

										// how do we know this selection is empty.... it actually cant be.. sooo?
										// we'll need to rely on the last instead of genning a new one,
										

										Event.current.Use ();
										if (onMouseUp != null) {
												onMouseUp (statelist.Last ());
										}
										break;

								}


						
						}
								
						break;





				case EventType.mouseDrag:
						var laststate = statelist.Last ();
						state = new DragState (laststate._connecting, true, Event.current.mousePosition, CurrentSelection ());
						statelist.Add (state);
						if (onMouseDrag != null) {
								onMouseDrag (state);
						}
						Event.current.Use ();
						break;







				case EventType.repaint:
						if (onGuiRepaint != null) {
								onGuiRepaint ();
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