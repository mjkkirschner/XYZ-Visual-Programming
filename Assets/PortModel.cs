using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using System;
using System.ComponentModel;

// <summary>
/// object representing an input port.
/// </summary>
public class PortModel : MonoBehaviour, Iinteractable, INotifyPropertyChanged
{

		GuiState GeneratedDragState;
		/// <summary>
		/// a tempconnector that is drawn while dragging.
		/// </summary>
		private ConnectorView tempconnector;

		public string NickName{ get; set; }

		public NodeModel Owner { get; set; }

		public int Index { get; set; }

		private float dist_to_camera;

		public Boolean IsConnected { get; set; }

		public List<ConnectorModel> connectors = new List<ConnectorModel> ();

		public delegate void PortConnectedHandler (object sender,EventArgs e);

		public delegate void PortDisconnectedHandler (object sender,EventArgs e);
		
		public event PortConnectedHandler PortConnected;
		public event PortConnectedHandler PortDisconnected;
		public event PropertyChangedEventHandler PropertyChanged;
	
		private void NotifyPropertyChanged (String info)
		{
				Debug.Log ("sending some property change notification");
				if (PropertyChanged != null) {
						PropertyChanged (this, new PropertyChangedEventArgs (info));
				}
		}

		protected virtual void OnPortConnected (EventArgs e)
		{
				if (PortConnected != null)
						PortConnected (this, e);
		}

		protected virtual void OnPortDisconnected (EventArgs e)
		{
				if (PortDisconnected != null)
						PortDisconnected (this, e);
		}

//		public void DestroyConnectors ()
//		{
//				if (Owner == null)
//						return;
//		
//				while (connectors.Any()) {
//						ConnectorModel connector = connectors [0];
//						Owner.Connectors.Remove (connector);
//						connector.NotifyConnectedPortsOfDeletion ();
//				}
//		}
	
		public void Connect (ConnectorModel connector)
		{
				connectors.Add (connector);
		
				//throw the event for a connection
				OnPortConnected (EventArgs.Empty);
		
				IsConnected = true;
		}
	
		public void Disconnect (ConnectorModel connector)
		{
				if (!connectors.Contains (connector))
						return;
				//throw the event for a connection
				OnPortDisconnected (EventArgs.Empty);
		
				//also trigger the model's connector deletion
				Owner.OnConnectorDeleted (connector);
		
				connectors.Remove (connector);
		
				//don't set back to white if
				//there are still connectors on this port
				if (connectors.Count == 0) {
						IsConnected = false;
				}
		
				Owner.ValidateConnections ();
		}


		public porttype PortType { get; set; }
		
		
		public enum porttype
		{

				input,
				output
			}
		;

		// Use this for initialization
		void Start ()
		{
				// guimanager - like a GUIcontroller
				var GuiManager = GameObject.FindObjectOfType<GuiTest> ();
		
				GuiManager.onMouseDown += new GuiTest.Mouse_Down (this.MyOnMouseDown);
				GuiManager.onMouseUp += new GuiTest.Mouse_Up (this.MyOnMouseUp);
				GuiManager.onMouseDrag += new GuiTest.Mouse_Drag (this.MyOnMouseDrag);
				GuiManager.onGuiRepaint += new GuiTest.GuiRepaint (this.onGuiRepaint);
				
				
				
		}
	
		void Update ()
		{
	
		}

		
		/// this handler is used to respond to changes on the node owner of this port
		// right now it just forwards the notification
		public void NodePropertyChangeEventHandler (object sender, EventArgs args)
		{
				NotifyPropertyChanged ("OwnerProperties");

		}

		public void init (NodeModel owner, int index, porttype type, string nickname = null)
		{
				

				this.Owner = owner;
				this.Index = index;
				this.PortType = type;
				if (nickname == null) {
						NickName = Owner.name;
						NickName = this.NickName + PortType.ToString () + Index.ToString ();
				}

		}



		//TODO MOVE THESE NEXT 3 menthods to a base class for all draggable items

		public static Vector3 ProjectCurrentDrag (float distance)
		{
		
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				var output = ray.GetPoint (distance);
				return output;
		}
	
		public Vector3 HitPosition (GameObject object_to_test)
		{
		
				// return the coordinate in world space where hit occured
		
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				var hit = new RaycastHit ();
				if (Physics.Raycast (ray, out hit)) {
						Debug.Log ("Mouse Down Hit " + hit.collider.gameObject);
						Debug.DrawRay (ray.origin, ray.direction * dist_to_camera, Color.red, 2.0f);
						if (hit.collider.gameObject == object_to_test) {
								return hit.point;
								// I was previoulsy returning hit.barycenter ... triangle?
						}
			
				}
				return object_to_test.transform.position;
		}
		/// <summary>
		/// check if we hit the current node to test, use the state to extract mouseposition
		/// </summary>
		/// <returns><c>true</c>, if test was hit, <c>false</c> otherwise.</returns>
		/// <param name="node_to_test">Node_to_test.</param>
		/// <param name="state">State.</param>
		public bool HitTest (GameObject item, GuiState state)
		{		
				// raycast from the camera through the mouse and check if we hit this current
				// node, if we do return true
		
				Ray ray = Camera.main.ScreenPointToRay (state.MousePos);
				var hit = new RaycastHit ();
				Debug.DrawRay (ray.origin, ray.direction * dist_to_camera);
		
				if (Physics.Raycast (ray, out hit)) {
						Debug.Log ("Mouse Down Hit  " + hit.collider.gameObject);
						Debug.DrawRay (ray.origin, ray.direction * dist_to_camera);
						if (hit.collider.gameObject == item) {
								return true;
						}
			
				}
				return false;
		}



		//event handlers
		// TODO THESE ARE CURRENTLY THE SAME AS NODEMODEL - need to be changed
		// and in some cases they should fire new events, like connection and unconnection
		// these events/handlers logic need to be worked through.
		// ports will be responsible for creating connectors

		public GuiState MyOnMouseUp (GuiState current_state)
		{		


				//handle this here for now:
				//destruction of temp connector
				if (tempconnector != null) {
						tempconnector.TemporaryGeometry.ForEach (x => UnityEngine.GameObject.DestroyImmediate (x));
				}

				//if we mouseUp over a port we need to check if we were connecting/dragging,
				// and then we'll instantiate a new connectorModel, the model will create it's own view
				// and the view will listen to its ports for property changes
				GuiState newState;
				Debug.Log ("Mouse up event handler called");
				
				//appears current_state is does not have correct MOUSEPOS... what else is incorrect...
				if (HitTest (this.gameObject, current_state)) {

						if ((current_state.Connecting)) {
								Debug.Log ("I" + this.NickName + " was just MouseUpedOn");
				
								newState = new GuiState (true, false, current_state.MousePos, new List<GameObject> (), false);
                                // if port is already connected then disconnect old port before creating new connector
                                if (this.IsConnected)
                                {
                                    Dis
                                }
								//instantiate new connector etc
								var realConnector = new GameObject ();
								realConnector.AddComponent<ConnectorModel> ();
								realConnector.GetComponent<ConnectorModel> ().init (current_state.Selection [0].GetComponent<PortModel> (), this);
                                this.Connect(realConnector.GetComponent<ConnectorModel>());

						} else {
								newState = new GuiState (false, false, current_state.MousePos, new List<GameObject> (), false);
						}
						GuiTest.statelist.Add (newState);
						return newState;
				} else {

						return null;

				}
		
		
		}
		
		//handler for dragging node event//
		public GuiState MyOnMouseDrag (GuiState current_state)
		{
				GuiState newState = current_state;
				Debug.Log ("drag even handler, on a port");
		
				if (current_state.Selection.Contains (this.gameObject)) {				// If doing a mouse drag with this component selected...
						// since we are in 3d space now, we need to conver this to a vector3...
						// for now just use the z coordinate of the first object
						// project from camera through mouse currently and use same distance
						Vector3 to_point = ProjectCurrentDrag (dist_to_camera);
						
						
						if (tempconnector != null) {
								tempconnector.TemporaryGeometry.ForEach (x => UnityEngine.GameObject.DestroyImmediate (x));
						}
						// since this is a port, we need to instantiate a new 
						//ConnectorView ( this is a temporary connector that we drag around in the UI)
						
						tempconnector = new ConnectorView (this.gameObject.transform.position, to_point);
						
						// move object to new coordinate
						newState = new GuiState (true, true, Input.mousePosition, current_state.Selection, false);
						GuiTest.statelist.Add (newState);
						Event.current.Use ();
			
				}
				return newState;
		
		
		}
		//handler for clicks
		public GuiState MyOnMouseDown (GuiState current_state)
		{
				Debug.Log ("mouse down event handler called");
				// check if this node was actually clicked on
				if (HitTest (this.gameObject, current_state)) {
						Debug.Log ("I" + this.name + " was just clicked");
						dist_to_camera = Vector3.Distance (this.transform.position, Camera.main.transform.position);
			
						Debug.Log (current_state);
			
						// check the dragstate from the GUI, either this is a double click
						// or a selection click
						// or possibly a click on nothing
						if (current_state.DoubleClicked == false) {
				
								// add this item to the current selection
								// update the drag state
								// store this drag state in the list of all dragstates
							
								// eventually we'll need to check if this port is an input or output
								
								List<GameObject> new_sel = (new List<GameObject> (current_state.Selection));
								new_sel.Add (this.gameObject);
								var newState = new GuiState (current_state.Connecting, current_state.Dragging, current_state.MousePos, new_sel, false); 
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

