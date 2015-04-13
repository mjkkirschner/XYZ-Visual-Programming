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
	public class ForLoop : ControlFlowDelegateNodeModel
	{
		
		protected override void Start()
		{
			base.Start();
			AddOutPutPort("i");
			AddInputPort("iterations");
			
			AddExecutionInputPort("start");
			
			AddExecutionOutPutPort("onIteration");
			AddExecutionOutPutPort("onIterated");
			
			
			CodePointer = CompiledNodeEval;
			Evaluator = this.gameObject.AddComponent<CsharpEvaluator>();
			viewPrefabs.Add("looptop");
			
		}
		
		protected override Dictionary<string,object> CompiledNodeEval(Dictionary<string,object> inputstate,Dictionary<string,object> intermediateOutVals)
		{
			var output = intermediateOutVals;
			var temprangeend = inputstate["iterations"];
			int rangeend = 0;

			if (temprangeend.GetType() == typeof(int))
			{
				rangeend = (int)temprangeend;
			}
			if(temprangeend.GetType() == typeof(double))
			{
				rangeend = (int)((double)(temprangeend));
			}

			IEnumerable range;
			if (rangeend < 0)

			{
				range = Enumerable.Range(0,Math.Abs(rangeend)).Select(x=> x*-1);
			}
			else
			{
				range = Enumerable.Range(0,rangeend);
			}


			foreach (var i in range)
			{
				output["i"] = i;
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