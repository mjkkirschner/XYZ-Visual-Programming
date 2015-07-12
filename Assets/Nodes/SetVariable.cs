using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;
using Nodeplay.Core;
using Nodeplay.UI;
using UnityEngine.UI;

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
			viewPrefabs = new List<string>(){ "VariableNodeBaseView"};
		}

		protected override Dictionary<string, object> CompiledNodeEval(Dictionary<string, object> inputstate, Dictionary<string, object> intermediateOutVals)
		{
			var output = intermediateOutVals;
			var variable = inputstate["variable"];
			var newvalue = inputstate["new_value"];

			((VariableReference)variable).Set(newvalue);

			output["variable_value"] = (variable as VariableReference).Get();
			(inputstate["done"] as Action).Invoke();
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
				
				var variableref = inputdict["variable"];
				var newvalue = inputdict["new_value"];

				((VariableReference)variableref).Set(newvalue);

				output["variable_value"] = (variableref as VariableReference).Get();

				var doneport = this.ExecutionOutputs.Where(x=>x.NickName == "done").FirstOrDefault();
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
			var UI = base.BuildSceneElements();
			UI.GetComponent<Renderer>().material.color = new Color(56.0f/256.0f,158.0f/256.0f,201.0f/256.0f);
			return UI;
			
		}

	}
}