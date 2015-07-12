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
		Action generatedAction;
		private List<NodeModel> visitedList = new List<NodeModel>();
		protected override void Start()
		{
			base.Start();
			
			AddExecutionOutPutPort("Update");
			
			CodePointer = CompiledNodeEval;
			Evaluator = this.gameObject.AddComponent<CsharpEvaluator>();
			viewPrefabs.Add("update");
			viewPrefabs.Add("execution");
			explicitGraphExecution.updaters.Add(this);
			generatedAction = generateFunc();
			//SubscribeToDownstreamNodes();
			
		}

		// we need to regen our delegate of the graph if the graph changes, so we should subscribe to the port connected
		// and disconnected events of the nodes, if these occur, we regenerate the delegate
		void SubscribeToDownstreamNodes(){

		if (this.ExecutionOutputs.First().IsConnected) {
			var visited = Nodeplay.UI.BoundingRenderer.BFS (this.ExecutionOutputs.First().connectors[0].PEnd.Owner.gameObject);
			foreach (var model in visited.Select(x=>x.GetComponent<NodeModel>()))
				{
					if (!visitedList.Contains(model))
					{
					model.PropertyChanged += HandlePropertyChanged;
					}
				}
			
			}
		}

		void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Ports")
			{
				generatedAction = generateFunc();
				SubscribeToDownstreamNodes();
			}
		}

		protected override Dictionary<string,object> CompiledNodeEval(Dictionary<string,object> inputstate,Dictionary<string,object> intermediateOutVals)
		{
			generatedAction.Invoke();
			return intermediateOutVals;
		}

		//protected override void Update()
		//{
		//	base.Update();
		//	CallOutPut(0,null);
		//}
		
	}
}