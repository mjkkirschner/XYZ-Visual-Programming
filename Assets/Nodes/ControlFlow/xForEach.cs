using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;
using System.Collections;
using Nodeplay.UI;
using System.Reflection.Emit;
using System.Reflection;


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
			viewPrefabs.Add("looptop");
			viewPrefabs.Add("iteration");

			
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

		public override Action generateFunc()
		{

			return new Action( ()=> {
				OnEvaluation();
			var inputstate = gatherInputPortData();
			var variableNames = inputstate.Select(x=>x.First).ToList();
			var variableValues = inputstate.Select(x=>x.Second).ToList();
			var inputdict = new Dictionary<string,object>();
			foreach (var variable in variableNames)
			{
				var index = variableNames.IndexOf(variable);
				inputdict[variable] =  variableValues[index];
			}

			var output = StoredValueDict;

			var tempinput1 = inputdict["input1"] as IEnumerable;
			
			foreach (var i in tempinput1) 
			{
				output["OUTPUT"] = i;
				this.ExecutionOutputs.Where(x=>x.NickName == "onIteration").First().connectors.FirstOrDefault().PEnd.Owner.generateFunc().Invoke();
			}
				var doneport = this.ExecutionOutputs.Where(x=>x.NickName == "onIterated").FirstOrDefault();

				if (doneport.connectors.Count>0)
				{
					doneport.connectors.First().PEnd.Owner.generateFunc().Invoke();
				}
				StoredValueDict = output;
				NotifyPropertyChanged("StoredValue");
				OnEvaluated();
			});
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