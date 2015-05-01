using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System;

namespace Nodeplay.UI
{

	public class BoundingRenderer : MonoBehaviour
	{

		public NodeModel Model;
		private bool initialzed = false;
		private List<Color> branchColors;
		private List<int> branchIndicies;
		public List<GameObject> tempgeos = new List<GameObject>();
		private List<NodeModel> visitedList = new List<NodeModel>();
		// Use this for initialization
		void Start()
		{
			Model = this.GetComponentInParent<NodeModel>();
			//TODO subscribing to wrong models, we need to subscribe to the most updated list of all nodes under us...
			//FOUND THOUGH BFS.
			Model.PropertyChanged += HandlePropertyChanged;
			Model.Evaluated +=  delegate {
				UpdateBounding();
			};
		}
		 void HandlePropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			//if (e.PropertyName == "Location")
			//{
				UpdateBounding();
			//}
		}


		public void initialze (List<int>indicies, List<Color> colors)
		{
			branchIndicies = indicies;
			branchColors = colors;
			initialzed = true;
		}

		public static List<GameObject> BFS(GameObject root)
		{
			// now can start actually traversing the graph
			List<GameObject> visited = new List<GameObject>();
			Queue<GameObject> Q = new Queue<GameObject>();


			Q.Enqueue(root);
			while (Q.Count > 0)
			{
				GameObject currentVertex = Q.Dequeue();
				var alloutputs = currentVertex.GetComponent<NodeModel>().ExecutionOutputs.SelectMany(x => x.connectors).ToList();

				foreach (var connector in alloutputs)
				{
					var head = connector.PEnd.Owner.gameObject;
					if (visited.Contains(head) == false)
					{
						Q.Enqueue(head);
					}

				}
				// look at BFS implementation again - CLRS ? colors? I am mutating verts too many times?
				// check how many times this loop is running with acounter
				visited.Add(currentVertex);

			}
			return visited;
		}


		public GameObject GenerateBounds(List<GameObject> toBound,Color matColor)
		{
			Vector3 center = Vector3.zero;
			var allrenderers = toBound.SelectMany(x => x.GetComponentsInChildren<MeshRenderer>()).ToList();
			var totalBounds = allrenderers[0].bounds;
			foreach (Renderer ren in allrenderers)
			{
				center = center + ren.gameObject.transform.position;
				totalBounds.Encapsulate(ren.bounds);

			}
			center = center / (allrenderers.Count);
			var xx = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			xx.transform.localScale = new Vector3(totalBounds.size.x, totalBounds.size.y, totalBounds.size.z);
			xx.transform.localPosition = totalBounds.center;
			xx.GetComponent<Collider>().enabled = false;
			xx.GetComponent<Renderer>().material = Resources.Load("NestingZone") as Material;
			xx.GetComponent<Renderer> ().material.color = new Color(matColor.r,matColor.g,matColor.b,.25f);
			return xx;
		}

		// Update is called once per frame

		void UpdateBounding()
		{
			if (initialzed) {
				if (tempgeos != null) {
					tempgeos.ForEach(x=>GameObject.DestroyImmediate(x));
				}
				//LOOP over each index and color
				foreach (var index in branchIndicies)
				{

				//generate the bounds of all gameobjects downstream from the first execution connector
				//could also search for the port with the correct naming convention
				if (Model.ExecutionOutputs [index].IsConnected) {
					var visited = BFS (Model.ExecutionOutputs [index].connectors[0].PEnd.Owner.gameObject);
					foreach (var model in visited.Select(x=>x.GetComponent<NodeModel>()))
						{
							//if we've visited this node before, then we dont need to 
							//subscribe to it's location move updates
							if (!visitedList.Contains(model))
							{
								model.PropertyChanged += HandlePropertyChanged;
							}
							visitedList.Add(model);
						}

					if (visited.Any (x => x.transform.hasChanged == true)) {

						tempgeos.Add(GenerateBounds (visited,branchColors[branchIndicies.IndexOf(index)]));
						}
					}
				}
			
			}

			//TODO we'll want some logic here to compare the visited list and the 
		}
	}

}