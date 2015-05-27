using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;

namespace Nodeplay.Nodes
{
	public class ListAdd : DelegateNodeModel
	{

		protected override void Start()
		{
			base.Start();

			AddInputPort("list",typeof(System.Collections.IList));
			AddInputPort("item to add");
			AddExecutionInputPort("start");
			AddExecutionOutPutPort("done");
			
			
			CodePointer = CompiledNodeEval;
			Evaluator = this.gameObject.AddComponent<CsharpEvaluator>();
		}
		
		protected override Dictionary<string,object> CompiledNodeEval(Dictionary<string,object> inputstate,Dictionary<string,object> intermediateOutVals)
		{
			var output = intermediateOutVals;
			var templist= inputstate["list"];
			var tempitem = inputstate["item to add"];


			((System.Collections.IList)templist).Add(tempitem);
			(inputstate["done"] as Action).Invoke();
			return output;

		}

	}
}
