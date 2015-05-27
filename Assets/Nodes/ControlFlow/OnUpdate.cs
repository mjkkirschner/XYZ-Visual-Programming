using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;
using System.Collections;
using Nodeplay.UI;

namespace Nodeplay.Nodes
{
	public class OnUpdate : ControlFlowDelegateNodeModel
	{
		
		protected override void Start()
		{
			base.Start();
			
			AddExecutionOutPutPort("Update");
			
			CodePointer = CompiledNodeEval;
			Evaluator = this.gameObject.AddComponent<CsharpEvaluator>();
			viewPrefabs.Add("update");
			viewPrefabs.Add("execution");
			explicitGraphExecution.updaters.Add(this);
			
		}
		
		protected override Dictionary<string,object> CompiledNodeEval(Dictionary<string,object> inputstate,Dictionary<string,object> intermediateOutVals)
		{
			var output = intermediateOutVals;
			(inputstate["Update"] as Action).Invoke();
			return output;
			
		}

		//protected override void Update()
		//{
		//	base.Update();
		//	CallOutPut(0,null);
		//}
		
	}
}