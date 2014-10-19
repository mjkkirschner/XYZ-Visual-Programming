using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Nodes;
using System;
/// <summary>
/// Node manager.
/// </summary>
public class NodeManager : MonoBehaviour
{

		public static GameObject DrawLine (Vector3 from, Vector3 to)
		{

				var linego = new GameObject ();
				var line = linego.AddComponent<LineRenderer> ();
				line.SetWidth (.1f, .1f);
				line.SetVertexCount (2);
				//line.material = aMaterial;
				line.renderer.enabled = true;
				line.SetPosition (0, from);
				line.SetPosition (1, to);
				return linego;
		}

		List<NodeModel> nodes = new List<NodeModel> ();
		List<GameObject> lines = new List<GameObject> ();

		// Use this for initialization
		void Start ()
		{
		
	
		}
	
		void Update ()
		{

				nodes = new List<NodeModel> (GameObject.FindObjectsOfType<NodeModel> ());

			
		}

		public GuiState onCanvasDoubleClick (GuiState currentstate)
		{

				//TODO needs to also subscribe new nodes to all the neccesary events.
				// Possibly these events should be inherited

				var mousePos = currentstate.MousePos;
				// this is basically reduce with a conditional either passing min or next, to find the min closest node
				// could replace with for loop...
				var closestNode = nodes.Aggregate ((min, next) => Vector3.Distance (min.transform.position, mousePos) < Vector3.Distance (next.transform.position, mousePos) ? min : next);
				
				// get distance to closest node
				
				var distToClosest = Vector3.Distance (Camera.main.transform.position, closestNode.transform.position);
													
				var creationPoint = NodeModel.ProjectCurrentDrag (distToClosest);
				
				var newnode = GameObject.CreatePrimitive (PrimitiveType.Cube);
				
				
				newnode.AddComponent<TestNodeType> ().name = "node" + Guid.NewGuid ().ToString ();
				newnode.transform.position = creationPoint;
				
				//Node.Selection = newnode.GetComponent<Node>();
				nodes.Add (newnode.GetComponent<NodeModel> ());
				
				Event.current.Use ();
				
				return new GuiState (false, false, mousePos, new List<GameObject> (){newnode}, false);
		}

		public void onGuiRepaint ()
		{
				

		}


		// Update is called once per frame
		//		void OnGUI ()
		//		{
		//
		//				if (Event.current.type == EventType.repaint) {
		//
		//						foreach (var line in lines) {
		//								Destroy (line);
		//
		//						}
		//						lines.Clear ();
		//
		//						foreach (Node node in nodes) {
		//
		//								foreach (Node target in node.Targets) {
		//										var DrawnLine = NodeManager.DrawLine (node.transform.position, target.transform.position);
		//										this.lines.Add (DrawnLine);
		//
		//
		//
		//								}
		//
		//
		//
		//						}
		//				}
		//				foreach (Node node in nodes)
		// {								// Handle all nodes
		//						node.OnGUI ();
		//				}
		//
		//
		//
		//				switch (Event.current.type) {
		//				case EventType.mouseUp:
		//							// If we had a mouse up event which was not handled by the nodes, clear our selection
		//						Node.Selection = null;
		//						Event.current.Use ();
		//						Debug.Log ("mouse up and no nodes handled this");
		//						break;
		//				case EventType.mouseDown:
		//						if (Event.current.clickCount == 2)
		// {								// If we double-click and no node handles the event, create a new node there
		//
		//								var mousePos = Event.current.mousePosition;
		//								var closestNode = nodes.Aggregate ((min, next) => Vector3.Distance (min.transform.position, mousePos) < Vector3.Distance (next.transform.position, mousePos) ? min : next);
		//
		//								// get distance to closest node
		//
		//								var distToClosest = Vector3.Distance (Camera.main.transform.position, closestNode.transform.position);
		//
		//								var creationPoint = Node.ProjectCurrentDrag (distToClosest);
		//
		//								var newnode = GameObject.CreatePrimitive (PrimitiveType.Cube);
		//
		//
		//								newnode.AddComponent<Node> ().name = "node" + (nodes.Count).ToString ();
		//								newnode.transform.position = creationPoint;
		//
		//								//Node.Selection = newnode.GetComponent<Node>();
		//								nodes.Add (newnode.GetComponent<Node> ());
		//
		//								Event.current.Use ();
		//
		//
		//
		//						}
		//						break;
		//				}
		//
		//
		//
		//
		//
		//		}
}
