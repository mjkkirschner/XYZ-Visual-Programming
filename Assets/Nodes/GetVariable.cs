using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;
using Nodeplay.Core;

namespace Nodeplay.Nodes
{
	public class GetVariable : DelegateNodeModel
	{

		protected override void Start()
		{
			base.Start();

			AddInputPort("variable");
			AddExecutionInputPort("start");
			AddOutPutPort("variable_value");
			AddExecutionOutPutPort("done");

			CodePointer = CompiledNodeEval;
			Evaluator = this.gameObject.AddComponent<CsharpEvaluator>();
		}

		protected override Dictionary<string, object> CompiledNodeEval(Dictionary<string, object> inputstate, Dictionary<string, object> intermediateOutVals)
		{
			var output = intermediateOutVals;
			var variable = inputstate["variable"];

			var x = ((VariableReference)variable).Get();

			output["variable_value"] = x;
			(inputstate["done"] as Delegate).DynamicInvoke();
			return output;

		}

	}
}