using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;
using Nodeplay.UI;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Nodeplay.Nodes
{
	public class Inspector : DelegateNodeModel
	{

		protected override void Start()
		{
			base.Start();

			AddInputPort("inputData");
			AddExecutionInputPort("startInspector");
			AddOutPutPort("passThrough");
			AddExecutionOutPutPort("EndInspector");


			CodePointer = CompiledNodeEval;
			Evaluator = this.gameObject.AddComponent<CsharpEvaluator>();
		}

		protected override Dictionary<string, object> CompiledNodeEval(Dictionary<string, object> inputstate, Dictionary<string, object> intermediateOutVals)
		{
			//clear output
			//TODO may need a way to do this on all nodes, keep track of all gameobjects generated during execution and
			//destroy them on next execution
			var childrenofVisualization = GetComponentInChildren<InspectorVisualization>().transform.Cast<Transform>().ToList();
			if (childrenofVisualization.Count > 0){
				childrenofVisualization.ForEach(x=>DestroyObject(x.gameObject));
			}
			var output = intermediateOutVals;
			var tempx = inputstate["inputData"];
			
			Debug.Log(tempx.GetType());
			//Debug.Break();
			//now here we will pass the input data to our visualization functions//
			//these can be here, will be on the visualziatin components on the node

			//for now we just passthrough data, but the visualization might redirect output
			// so this data might be assigned through whatever function runs on the component

			transform.root.GetComponentInChildren<InspectorVisualization>().PopulateTopLevel(tempx);

			output["passThrough"] = tempx;
			//this just calls the execution trigger
			(inputstate["EndInspector"] as Delegate).DynamicInvoke();
			return output;

		}


		public override GameObject BuildSceneElements ()
		{
			//for this node UI will add we'll add a InspectorVisualization component and call methods on it
			//when the node is evaluated

			GameObject UI = Instantiate(Resources.Load("NodeBaseView")) as GameObject;
			UI.transform.localPosition = this.gameObject.transform.position;
			UI.transform.parent = this.gameObject.transform;
			UI.renderer.material.color = Color.yellow;
			UI.AddComponent<InspectorVisualization>();
			//iterate all graphics casters and turn blocking on for 3d objects
			var allcasters = this.GetComponentsInChildren<GraphicRaycaster>().ToList();
			allcasters.ForEach(x=>x.blockingObjects = GraphicRaycaster.BlockingObjects.ThreeD);
			
			UI.AddComponent<Light>().type = LightType.Point;
			UI.GetComponent<Light>().range = 35;
			UI.GetComponent<Light>().intensity = .15f;
			UI.GetComponent<Light>().color = Color.white;
			return UI;
		}

	}
}
