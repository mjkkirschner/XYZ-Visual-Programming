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

		
		List<NodeModel> nodes = new List<NodeModel> ();

		// Use this for initialization
		void Start ()
		{
		
	
		}
	
		void Update ()
		{

				nodes = new List<NodeModel> (GameObject.FindObjectsOfType<NodeModel> ());

			
		}
        //todo will need to block out/implment functions to create nodes,connector etc with no GUI interaction
        // for loading
        /// <summary>
        /// this method instantiates a node of any type that inherits from node model
        /// it creates a gameobject to host the NodeModel by parsing the type parameters
        /// </summary>
        /// <typeparam name="NT"></typeparam>
        /// <returns></returns>
       
        public GameObject InstantiateNode<NT>(NT node) where NT: NodeModel
        {
            return null;
        }


		public GuiState onCanvasDoubleClick (GuiState currentstate)
		{

				
				var mousePos = currentstate.MousePos;
				// this is basically reduce with a conditional either passing min or next, to find the min closest node
				// could replace with for loop...
				var closestNode = nodes.Aggregate ((min, next) => Vector3.Distance (min.transform.position, mousePos) < Vector3.Distance (next.transform.position, mousePos) ? min : next);
				// get distance to closest node
				var distToClosest = Vector3.Distance (Camera.main.transform.position, closestNode.transform.position);							
				var creationPoint = BaseView.ProjectCurrentDrag (distToClosest);

                //todo creation of a new node or element needs to be redesigned - 
                // process will be in general - 
                // create an empty gameobject
                // add a Model to it
                // Model will create a view which will also be added to the root GO
                // the view will call a method on the model to construct UI elements
                // which will be added to the scene and form some tree structure under the root

				var newnode = GameObject.CreatePrimitive (PrimitiveType.Cube);
				
				
				newnode.AddComponent<TestNodeType> ().name = "node" + Guid.NewGuid ().ToString ();
				newnode.transform.position = creationPoint;
                newnode.AddComponent<NodeView>();
				
				nodes.Add (newnode.GetComponent<NodeModel> ());
				
				Event.current.Use ();
				
				return new GuiState (false, false, mousePos, new List<GameObject> (){newnode}, false);
		}

		public void onGuiRepaint ()
		{
				

		}


		
}
