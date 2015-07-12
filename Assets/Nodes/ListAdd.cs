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
				var templist= inputdict["list"];
				var tempitem = inputdict["item to add"];
				
				((System.Collections.IList)templist).Add(tempitem);

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


	}
}
