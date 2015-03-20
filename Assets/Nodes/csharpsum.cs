using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;

namespace Nodeplay.Nodes
{
	public class csharpsum : DelegateNodeModel
	{

		protected override void Start()
		{
			base.Start();

			AddInputPort("x");
			AddInputPort("y");
			AddExecutionInputPort("start");
			AddOutPutPort("OUTPUT");
			AddExecutionOutPutPort("done");
			
			
			CodePointer = CompiledNodeEval;
			Evaluator = this.gameObject.AddComponent<CsharpEvaluator>();
		}
		
		protected override Dictionary<string,object> CompiledNodeEval(Dictionary<string,object> inputstate,Dictionary<string,object> intermediateOutVals)
		{
			var output = intermediateOutVals;
			var tempx = inputstate["x"];
			var tempy = inputstate["y"];

			Debug.Log(tempx.GetType());
			Debug.Log(tempy);
			var sum = (int)tempx + (int)tempy;
			output["OUTPUT"] = sum;
			(inputstate["done"] as Action).Invoke();
			return output;

		}

	}
}
