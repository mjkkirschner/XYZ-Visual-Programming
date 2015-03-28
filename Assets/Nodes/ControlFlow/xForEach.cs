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
	public class xForEach : ControlFlowDelegateNodeModel
	{
		
		protected override void Start()
		{
			base.Start();
			AddOutPutPort("OUTPUT");
			AddInputPort("input1");
			
			AddExecutionInputPort("start");
			
			AddExecutionOutPutPort("onIteration");
			AddExecutionOutPutPort("onIterated");
			
			
			CodePointer = CompiledNodeEval;
			Evaluator = this.gameObject.AddComponent<CsharpEvaluator>();
		}
		
		protected override Dictionary<string,object> CompiledNodeEval(Dictionary<string,object> inputstate,Dictionary<string,object> intermediateOutVals)
		{
			var output = intermediateOutVals;
			var tempinput1 = inputstate["input1"] as IEnumerable;
			
			foreach (var i in tempinput1) 
			{
				output["OUTPUT"] = i;
				(inputstate["onIteration"] as Action).Invoke();
			}

			(inputstate["onIterated"] as Action).Invoke();
			return output;
			
		}

		public override GameObject BuildSceneElements()
		{
			var tempUI = base.BuildSceneElements();
			tempUI.AddComponent<BoundingRenderer>();
			tempUI.GetComponent<BoundingRenderer> ().initialze (new List<int> (){0}, new List<Color>() {Color.cyan});
			return tempUI;
			
		}
		
	}
}