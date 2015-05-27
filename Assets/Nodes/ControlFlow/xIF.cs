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
	public class xIF : ControlFlowDelegateNodeModel
	{
		
		protected override void Start()
		{
			
			base.Start();
			
			AddInputPort("test");
			
			AddExecutionInputPort("startTest");
			
			AddExecutionOutPutPort("TrueBranch");
			AddExecutionOutPutPort("ElseBranch");
			AddExecutionOutPutPort("done");
			
			CodePointer = CompiledNodeEval;
			Evaluator = this.gameObject.AddComponent<CsharpEvaluator>();
			viewPrefabs.Add("conditional");

			
		}
		
		protected override Dictionary<string,object> CompiledNodeEval(Dictionary<string,object> inputstate,Dictionary<string,object> intermediateOutVals)
		{
			var output = intermediateOutVals;
			var temptest = (bool)inputstate["test"];
			
			if(temptest)
			{
				(inputstate["TrueBranch"] as Action).Invoke();
			}

			else{
				(inputstate["ElseBranch"] as Action).Invoke();
			}

			(inputstate["done"] as Action).Invoke();
			return output;
			
		}
		
		public override GameObject BuildSceneElements()
		{
			var tempUI = base.BuildSceneElements();
			tempUI.AddComponent<BoundingRenderer>();
			tempUI.GetComponent<BoundingRenderer> ().initialze (new List<int> (){0,1}, 
			new List<Color>() {Color.cyan,new Color(.7f,.4f,.1f)});
			return tempUI;
			
		}
		
	}
}