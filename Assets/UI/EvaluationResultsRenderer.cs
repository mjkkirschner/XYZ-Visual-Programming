using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using Pathfinding.Serialization.JsonFx;

namespace Nodeplay.UI
{
	//
	//this component watches for evaluation events on it's owner Nodemodel and creates a gameobject representation of the
	//the evaluation results for a particular eval of the node
	public class EvaluationResultsRenderer : MonoBehaviour
	{

		public NodeModel Model;
		private List<GameObject> evaluationResults = new List<GameObject>();

		// Use this for initialization
		void Start()
		{
			Model = this.transform.root.GetComponentInChildren<NodeModel>();
			Model.Evaluated += HandleEvaluated;
			Model.explicitGraphExecution.GraphEvaluationStarted += HandleGraphEvaluationStarted;
		}

		//when the graph starts being evaluated, cleanup all the past results we builts
		void HandleGraphEvaluationStarted ()
		{
			evaluationResults.ForEach(x=>GameObject.Destroy(x));
			evaluationResults.Clear();
		}

		//when the nodemodel is finished evaluating, then build a record of this evaluation
		//we'll store the resultant outputs, from the storedValueDict on the nodemodel and
		//render these in space near the node.
		void HandleEvaluated (object sender, EventArgs e)
		{


			var keys = Model.Outputs.Select(x=>x.NickName);
			var vals =keys.Select(x=>Model.StoredValueDict[x]).ToList();

			//now foreach of these we want to build some representation in 3d, but it needs
			//to be obvious they outputs from the same eval on the same node.... we can just
			//build a new OutputPair... or simply just grab the output pair and save it... this
			//is not useful for geometry.... what about adding all of these items to a list, and
			// building an inspector, and inspecting that list... might work nicely, will show geometry...
			var evalResultParent = GameObject.CreatePrimitive(PrimitiveType.Cube);
			evalResultParent.GetComponent<Renderer>().material.color  = Color.red;
			evalResultParent.AddComponent<BaseModel>();
			evalResultParent.transform.SetParent(Model.transform,false);
			//now move the parent in x from the nodemodel...
			evalResultParent.transform.Translate(
				new Vector3(evaluationResults.Count+evalResultParent.GetComponent<Renderer>().bounds.size.x*3+1 * -1,0,0));
			                                                                                  

			evalResultParent.AddComponent<InspectorVisualization>();
			evaluationResults.Add(evalResultParent);
			evalResultParent.GetComponent<InspectorVisualization>().PopulateTopLevel(vals,0);

		}

	}
	
}