using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;
using Nodeplay.Core;

namespace Nodeplay.Nodes
{
	public class SetVariable : DelegateNodeModel
	{

		protected override void Start()
		{
			base.Start();

			AddInputPort("variable");
			AddInputPort("new_value");
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
			var newvalue = inputstate["new_value"];

			((VariableReference)variable).Set(newvalue);

			output["variable_value"] = (variable as VariableReference).Get();
			(inputstate["done"] as Delegate).DynamicInvoke();
			return output;

		}

	}
}