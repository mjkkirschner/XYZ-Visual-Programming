using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using Pathfinding.Serialization.JsonFx;

namespace Nodeplay.UI
{
	/// <summary>
	/// this class positions nodes relative to their parent nodes, both from execution and data connectors
	/// this class watches for propertychanged events on the nodemodel/nodeview and updates only when needed
	/// this position is a suggestion, and should not be strictly enforced...this may be accomplished by disabling this
	/// component on a per node basis
	/// </summary>
	public class PositionNodeRelativeToParents : MonoBehaviour
	{

		public NodeModel Model;

		// Use this for initialization
		void Start()
		{
			Model = this.GetComponentInParent<NodeModel>();

			//scan the model and find all input ports, we'll subscribe to port connected events for all input ports
			// and update the position when ports are first connected...
			//TODO will want to concat this with executon inputs
			foreach(var port in Model.Inputs.Concat(Model.ExecutionInputs.Cast<PortModel>()))
			{
				port.PortConnected += OnPortConnect;
			}

		}

		protected void OnPortConnect(object sender, ConnectorModel e)
		{
			if (Model.Inputs.Count>0 && Model.Inputs.Any(x=>x.IsConnected) || Model.ExecutionInputs.Count>0 && Model.ExecutionInputs.Any(x=>x.IsConnected) )
			    {
			var newposTuple = tryCalculateNodePositon(getExecutionParents(),getDataParents());
				//some kind of null check?
				if( newposTuple.Second != false)
				{
				StartCoroutine(slowmove(this.transform.position,newposTuple.First,2));
				}
			}

		}
			
		private List<GameObject> getExecutionParents()
		{
			var inputparents  = this.Model.ExecutionInputs.SelectMany(x => x.connectors.Select(y=>y.PStart.Owner)).ToList();
			return inputparents.Select(x=>x.gameObject).ToList();
		}

		private List<GameObject> getDataParents()
		{
			var inputparents  = this.Model.Inputs.SelectMany(x => x.connectors.Select(y=>y.PStart.Owner)).ToList();
			return inputparents.Select(x=>x.gameObject).ToList();
		}

		private Vector3 calculateCentroid (List<Vector3>points)
		{
			Vector3 center = Vector3.zero;
			foreach (var point in points)
			{
				center = center + point;
			}

			if (points.Count<1)
			{
				Debug.Log("can't find centroid, no points");
				return Vector3.zero;
			}
			center = center / (points.Count);
			return center;
		}

		private Tuple<Vector3,bool> tryCalculateNodePositon(List<GameObject> execParents,List<GameObject> dataParents)
		{
			//first do a check on the lengths of the lists compared to the number of inputs the node has
			//for instance, the start node has no execution parents, so don't break if execparents is <1
			//but in other cases we want to 
			if (execParents.Count == 0 && Model.ExecutionInputs.Count>0)
			{
				return Tuple.New(Vector3.zero,false);
			}
		
			if (dataParents.Count == 0 && Model.Inputs.Count>0)
			{
				return Tuple.New(Vector3.zero,false);
			}
			//first calculate the 2 parent centroids
			var execcentroid = calculateCentroid(execParents.Select(x=>x.transform.position).ToList());
			var datacentroid = calculateCentroid(dataParents.Select(x=>x.transform.position).ToList());
			var rendererbnds = GetAllSubRendererBounds(new List<GameObject>(){this.gameObject});
			var newpos = execcentroid;
			// then push the the curret node strongly towards the centroid of the data parents + some amount in x and - y
			//[p]  
			//[p]
			//[p]    [c]

			Debug.Log(execcentroid);
			Debug.Log(datacentroid);
			Debug.Log(rendererbnds);




			//if this node has no datainputs then just calculate the execution connector
			//if (dataParents.Count >0)
			//{
			//	newpos = datacentroid + new Vector3(0.0f,(rendererbnds.size.y * 2)*-1.0f,(rendererbnds.size.z * 2));
			//}
			// now modify newpos by moving in the x vector towards the execution centroid

			var vectoExec = newpos - (execcentroid + new Vector3(rendererbnds.size.x * 15,0,rendererbnds.size.z * -5));
			newpos = newpos + vectoExec/2;


			//TODO, this algorithm will produce nodes exactly on top of each other, 3 ideas, 
			// 1. introduce some randomness into the position calculation, 
			// 2. count how many downstream nodes there are and use this for the calculation (should be easy, just look at number of connectors)
			// 3. attach colliders to nodes so nodes just wont interset each other
			// 4. do a check at the end and just push nodes apart that are too close...

			//none of this may matter if execution connectors also weight nodes to move apart


			//now strictly enforce the z position (forward, whatever that might be, into the screen) of the execution data connectors (+ x)
			// this means that in z code 

			return Tuple.New(newpos,true);
		}

		private Bounds GetAllSubRendererBounds(List<GameObject> toBound)
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
			return totalBounds;
		}

		protected virtual IEnumerator slowmove(Vector3 frompos,Vector3 topos, float duration)
		{
			for (float f = 0; f <= duration; f = f + Time.deltaTime)
			{
				this.transform.position = Vector3.Lerp(frompos, topos, f);
				yield return null;
				
			}
			this.transform.position = topos;
		}

	}
	
}