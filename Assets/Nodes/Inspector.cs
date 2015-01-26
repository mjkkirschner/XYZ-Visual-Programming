using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;

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
			var output = intermediateOutVals;
			var tempx = inputstate["inputData"];
			
			Debug.Log(tempx.GetType());
			
			//now here we will pass the input data to our visualization functions//
			//these can be here, will be on the visualziatin components on the node

			//for now we just passthrough data, but the visualization might redirect output
			// so this data might be assigned through whatever function runs on the component
			output["passThrough"] = tempx;
			//this just calls the execution trigger
			(inputstate["EndInspector"] as Delegate).DynamicInvoke();
			return output;

		}

	}
}
