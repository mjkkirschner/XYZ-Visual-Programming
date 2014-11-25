using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Nodes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class NodeManager : MonoBehaviour, IPointerClickHandler
{

		
		List<NodeModel> nodes = new List<NodeModel> ();

		// Use this for initialization
        void Start()
        {
            //TODO REMOVE THIS TEST CODE
            foreach (var i in Enumerable.Range(0, 2).ToList())
            {
                InstantiateNode<Number>(Vector3.zero);
            }

		InstantiateNode<ForLoopTest>(new Vector3(1,1,1));
		InstantiateNode<DebugLogTest>(new Vector3(2,2,2));

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
       
        public GameObject InstantiateNode<NT>(Vector3 point) where NT: NodeModel
        {
            var newnode = new GameObject();
            newnode.transform.position = point;
            newnode.AddComponent<NT>().name = "node" + Guid.NewGuid().ToString();
            nodes.Add(newnode.GetComponent<NodeModel>());
            return newnode;
        }


		public void OnPointerClick(PointerEventData eventData)
		{
            if (eventData.clickCount != 2)
            {
                return;
            }
            var creationPoint = Vector3.zero;
            var mousePos = eventData.pressPosition;
            if (nodes.Count > 0)
            {
                
                // this is basically reduce with a conditional either passing min or next, to find the min closest node
                // could replace with for loop...
                var closestNode = nodes.Aggregate((min, next) => Vector3.Distance(min.transform.position, mousePos) < Vector3.Distance(next.transform.position, mousePos) ? min : next);
                // get distance to closest node
                var distToClosest = Vector3.Distance(Camera.main.transform.position, closestNode.transform.position);
                 creationPoint = BaseView<NodeModel>.ProjectCurrentDrag(distToClosest);
            }
           
                //todo creation of a new node or element needs to be redesigned - 
                // process will be in general - 
                // create an empty gameobject
                // add a Model to it
                // Model will create a view which will also be added to the root GO
                // the view will call a method on the model to construct UI elements
                // which will be added to the scene and form some tree structure under the root

                var newnode = InstantiateNode<TestNodeType>(creationPoint);
				
		}

	

		
}
