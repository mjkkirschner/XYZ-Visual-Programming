using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace Nodeplay.UI
{

	public class BoundingRenderer : MonoBehaviour
	{

		public NodeModel Model;

		public GameObject tempgeo;
		// Use this for initialization
		void Start()
		{
			Model = this.GetComponentInParent<NodeModel>();
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


		public GameObject GenerateBounds(List<GameObject> toBound)
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
			return xx;
		}

		// Update is called once per frame
		void Update()
		{

			if (tempgeo != null)
			{
				GameObject.Destroy(tempgeo);
			}
			//generate the bounds of all gameobjects downstream from the first execution connector
			//could also search for the port with the correct naming convention
			if (Model.ExecutionOutputs[0].IsConnected)
			{
				tempgeo = GenerateBounds(BFS(Model.ExecutionOutputs[0].connectors[0].PEnd.Owner.gameObject));
			}
		}
	}

}